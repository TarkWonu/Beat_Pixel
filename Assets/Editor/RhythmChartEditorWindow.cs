#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class RhythmChartEditorWindow : EditorWindow
{
    private RhythmChart chart;

    // 레이아웃
    private const float HeaderH = 92f;
    private const float LanesH = 140f;
    private const float LaneH = LanesH / 2f;

    // 타임라인
    private float zoom = 1f;              // 0.5~3.0
    private float pixelsPerBeat = 90f;    // 기본 스케일
    private float scrollX = 0f;           // 가로 스크롤(px) 

    // 편집 상태
    private int dragIndex = -1;
    private bool isDragging = false;

    // 목록 스크롤
    private Vector2 listScroll;

    // ===== 재생(AudioUtil) =====
    private bool isPlaying = false;
    private double playStartEditorTime;   // EditorApplication.timeSinceStartup
    private float playOffsetSec = 0f;     // Pause 후 이어재생/Seek용

    private static readonly Type AudioUtilType =
        typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");

    // Unity 버전에 따라 시그니처가 다를 수 있어 가장 흔한 형태로 호출
    // PlayPreviewClip(AudioClip clip, int startSample, bool loop)
    private static MethodInfo PlayPreviewClipMethod =>
        AudioUtilType?.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null,
            new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);

    private static MethodInfo StopAllPreviewClipsMethod =>
        AudioUtilType?.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);

    [MenuItem("Tools/Rhythm/Rhythm Chart Editor")]
    public static void Open() => GetWindow<RhythmChartEditorWindow>("Rhythm Editor");

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        StopPreview();
    }

    private void OnEditorUpdate()
    {
        if (isPlaying) Repaint();
    }

    private void OnGUI()
    {
        DrawTopPanel();

        if (chart == null)
        {
            EditorGUILayout.HelpBox("RhythmChart 에셋을 선택하세요. (Create > Rhythm > Rhythm Chart)", MessageType.Info);
            return;
        }

        Rect timelineRect = new Rect(10, HeaderH, position.width - 20, LanesH);

        DrawTimeline(timelineRect);
        HandleTimelineInput(timelineRect);

        GUILayout.Space(HeaderH + LanesH + 10);
        DrawNotesList();

        if (GUI.changed)
            EditorUtility.SetDirty(chart);
    }

    // ===================== 상단 패널 =====================
    private void DrawTopPanel()
    {
        GUILayout.Space(6);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Chart", GUILayout.Width(40));
            chart = (RhythmChart)EditorGUILayout.ObjectField(chart, typeof(RhythmChart), false);

            GUILayout.FlexibleSpace();

            if (chart != null)
            {
                using (new EditorGUI.DisabledScope(isPlaying))
                {
                    chart.clip = (AudioClip)EditorGUILayout.ObjectField("Audio", chart.clip, typeof(AudioClip), false, GUILayout.Width(320));
                }

                chart.bpm = EditorGUILayout.FloatField("BPM", Mathf.Max(1f, chart.bpm), GUILayout.Width(180));
                chart.snapDiv = EditorGUILayout.IntPopup("Snap", chart.snapDiv,
                    new[] { "1/1", "1/2", "1/4", "1/8", "1/15","1/16" },
                    new[] { 1, 2, 4, 8, 15,16 },
                    GUILayout.Width(360));

                zoom = EditorGUILayout.Slider("Zoom", zoom, 0.5f, 3.0f, GUILayout.Width(260));
            }
        }

        GUILayout.Space(6);

        // Transport
        using (new EditorGUILayout.HorizontalScope("box"))
        {
            bool canPlay = chart != null && chart.clip != null;

            using (new EditorGUI.DisabledScope(!canPlay))
            {
                if (!isPlaying)
                {
                    if (GUILayout.Button("▶ Play", GUILayout.Height(26), GUILayout.Width(70)))
                        PlayPreview(playOffsetSec);
                }
                else
                {
                    if (GUILayout.Button("⏸ Pause", GUILayout.Height(26), GUILayout.Width(70)))
                        PausePreview();
                }

                if (GUILayout.Button("⏹ Stop", GUILayout.Height(26), GUILayout.Width(70)))
                    StopPreview();
            }

            GUILayout.Space(8);

            float t = GetSongTimeSec();
            float secLen = (chart != null && chart.clip != null) ? chart.clip.length : 0f;

            GUILayout.Label($"Time: {t:0.000}s / {secLen:0.000}s", GUILayout.Width(220));
            GUILayout.Label($"Beat: {SecToBeat(t):0.000}", GUILayout.Width(140));

            GUILayout.FlexibleSpace();

            // Seek Slider
            using (new EditorGUI.DisabledScope(chart == null || chart.clip == null))
            {
                float newT = GUILayout.HorizontalSlider(t, 0f, secLen, GUILayout.Width(320));
                if (Math.Abs(newT - t) > 0.0001f)
                {
                    Seek(newT);
                }
            }

            if (GUILayout.Button("Sort", GUILayout.Width(60)))
            {
                if (chart == null) return;
                Undo.RecordObject(chart, "Sort Notes");
                chart.notes = chart.notes.OrderBy(n => n.beat).ThenBy(n => n.type).ToList();
                EditorUtility.SetDirty(chart);
            }

            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                if (chart == null) return;
                if (EditorUtility.DisplayDialog("Clear All?", "모든 노트를 삭제할까요?", "Yes", "No"))
                {
                    Undo.RecordObject(chart, "Clear Notes");
                    chart.notes.Clear();
                    dragIndex = -1;
                    isDragging = false;
                    EditorUtility.SetDirty(chart);
                }
            }
        }

        // 분리선
        EditorGUI.DrawRect(new Rect(0, HeaderH - 2, position.width, 1), new Color(0, 0, 0, 0.25f));
    }

    // ===================== 타임라인 그리기 =====================
    private void DrawTimeline(Rect rect)
    {
        // 배경
        EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.12f, 1f));

        // 레인 배경
        Rect laneARect = new Rect(rect.x, rect.y, rect.width, LaneH);
        Rect laneBRect = new Rect(rect.x, rect.y + LaneH, rect.width, LaneH);
        EditorGUI.DrawRect(laneARect, new Color(0, 0, 0, 0.08f));
        EditorGUI.DrawRect(laneBRect, new Color(0, 0, 0, 0.14f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y + LaneH, rect.width, 1), new Color(1, 1, 1, 0.12f));

        // 레인 라벨
        GUI.Label(new Rect(rect.x + 6, rect.y + 6, 30, 20), "A", EditorStyles.boldLabel);
        GUI.Label(new Rect(rect.x + 6, rect.y + LaneH + 6, 30, 20), "B", EditorStyles.boldLabel);

        // ===== 촘촘한 그리드 =====
        float ppb = pixelsPerBeat * zoom;
        float left = rect.x - scrollX;
        float right = rect.xMax;

        int div = Mathf.Max(1, chart.snapDiv);
        const int beatsPerBar = 4;

        // 화면에 보이는 최소/최대 beat
        float firstBeat = scrollX / ppb;
        float lastBeat = (scrollX + rect.width) / ppb;

        // sub-step 단위(1/div)
        int kStart = Mathf.FloorToInt(firstBeat * div) - 2;
        int kEnd = Mathf.CeilToInt(lastBeat * div) + 2;

        for (int k = kStart; k <= kEnd; k++)
        {
            float beat = k / (float)div;
            if (beat < 0) continue;

            float x = rect.x + (beat * ppb) - scrollX;
            if (x < rect.x - 4 || x > right + 4) continue;

            // 강조 규칙
            // - 마디선: 굵고 진함
            // - 1비트: 중간
            // - 1/2, 1/4 ...: 얇고 약함
            bool isBar = (k % (beatsPerBar * div) == 0);
            bool isBeat = (k % div == 0);

            float w = isBar ? 2f : (isBeat ? 1.6f : 1f);
            float a = isBar ? 0.38f : (isBeat ? 0.24f : 0.12f);

            // snapDiv가 커질수록 너무 빽빽해 보이니 아주 미세선은 더 약하게
            if (!isBeat)
            {
                // 1/2는 조금 더 보이게
                if (div >= 2 && (k % (div / 2) == 0)) a = 0.16f;
                // 1/4는 중간
                if (div >= 4 && (k % (div / 4) == 0)) a = 0.14f;
                // 더 미세(1/8~)는 약하게
                if (div >= 8 && (k % (div / 8) == 0)) a = 0.10f;
            }

            EditorGUI.DrawRect(new Rect(x, rect.y, w, rect.height), new Color(1, 1, 1, a));

            if (isBar)
            {
                int barIndex = (Mathf.RoundToInt(beat) / beatsPerBar) + 1;
                GUI.Label(new Rect(x + 4, rect.y + rect.height - 18, 60, 18),
                    $"Bar {barIndex}",
                    new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = new Color(1, 1, 1, 0.6f) }
                    });
            }
        
        }

        // ===== 노트 =====
        for (int i = 0; i < chart.notes.Count; i++)
        {
            float width = 12;
            var n = chart.notes[i];
            float x = rect.x + (n.beat * ppb) - scrollX;
            if (x < rect.x - 20 || x > rect.xMax + 20) continue;

            float yCenter = (n.type == NoteType.A)
                ? rect.y + LaneH * 0.5f
                : rect.y + LaneH + LaneH * 0.5f;
            if (n.isLongNote)
            {
                
                width = ppb * Mathf.Max(0.1f, n.longNoteSize); // 최소 길이 확보
            }

            Rect noteRect = new Rect(x, yCenter - 10, width, 20);

            Color c = (n.type == NoteType.A)
                ? new Color(0.35f, 0.85f, 0.35f, 1f)
                : new Color(0.35f, 0.6f, 0.95f, 1f);

            if (i == dragIndex) c = Color.Lerp(c, Color.white, 0.35f);

            EditorGUI.DrawRect(noteRect, c);
            EditorGUI.DrawRect(new Rect(noteRect.x, noteRect.y, noteRect.width, 1), new Color(1, 1, 1, 0.35f));
            EditorGUI.DrawRect(new Rect(noteRect.x, noteRect.yMax - 1, noteRect.width, 1), new Color(0, 0, 0, 0.25f));
        }

        // ===== 재생 헤드 =====
        if (chart.clip != null)
        {
            float t = GetSongTimeSec();
            float headBeat = SecToBeat(t);
            float headX = rect.x + (headBeat * ppb) - scrollX;

            if (headX >= rect.x && headX <= rect.xMax)
            {
                EditorGUI.DrawRect(new Rect(headX, rect.y, 2f, rect.height), new Color(1f, 0.2f, 0.2f, 0.75f));
            }
        }

        // 안내 문구
        GUI.Label(new Rect(rect.xMax - 420, rect.y + 6, 414, 18),
            "LMB: add/select  |  Drag: move  |  RMB: delete  |  Ctrl+LMB: seek  |  Wheel: scroll  |  Shift+Wheel: zoom",
            new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.UpperRight,
                normal = { textColor = new Color(1, 1, 1, 0.55f) }
            });
    }

    // ===================== 입력 처리 =====================
    private void HandleTimelineInput(Rect rect)
    {
        if (chart == null) return;

        Event e = Event.current;
        bool hover = rect.Contains(e.mousePosition);

        // Wheel: scroll / Shift+Wheel: zoom
        if (hover && e.type == EventType.ScrollWheel)
        {
            if (e.shift)
            {
                float oldZoom = zoom;
                zoom = Mathf.Clamp(zoom + (-e.delta.y * 0.02f), 0.5f, 3.0f);

                float ppbOld = pixelsPerBeat * oldZoom;
                float ppbNew = pixelsPerBeat * zoom;

                float mouseBeat = (scrollX + (e.mousePosition.x - rect.x)) / ppbOld;
                float newScrollX = mouseBeat * ppbNew - (e.mousePosition.x - rect.x);
                scrollX = Mathf.Max(0f, newScrollX);

                e.Use();
                Repaint();
                return;
            }
            else
            {
                scrollX = Mathf.Max(0f, scrollX + e.delta.y * 25f);
                e.Use();
                Repaint();
                return;
            }
        }

        // Ctrl+좌클릭: Seek
        if (hover && e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            float beat = MouseToBeat(rect, e.mousePosition.x);
            beat = SnapBeat(beat);

            float sec = BeatToSec(beat);
            Seek(sec);

            e.Use();
            Repaint();
            return;
        }

        // 좌클릭: 노트 선택(드래그) or 빈곳 추가
        if (hover && e.type == EventType.MouseDown && e.button == 0 && !e.control)
        {
            dragIndex = PickNoteIndex(rect, e.mousePosition);
            if (dragIndex >= 0)
            {
                isDragging = true;
                GUI.FocusControl(null);
                e.Use();
                return;
            }

            // 빈 곳: 추가
            Undo.RecordObject(chart, "Add Note");
            NoteType type = MouseToLane(rect, e.mousePosition.y);
            float beat = SnapBeat(MouseToBeat(rect, e.mousePosition.x));

            chart.notes.Add(new RhythmNote { beat = Mathf.Max(0f, beat), type = type });
            chart.notes = chart.notes.OrderBy(n => n.beat).ThenBy(n => n.type).ToList();

            EditorUtility.SetDirty(chart);
            e.Use();
            Repaint();
            return;
        }

        // 우클릭: 삭제
        if (hover && e.type == EventType.MouseDown && e.button == 1)
        {
            int idx = PickNoteIndex(rect, e.mousePosition);
            if (idx >= 0)
            {
                Undo.RecordObject(chart, "Remove Note");
                chart.notes.RemoveAt(idx);
                dragIndex = -1;
                isDragging = false;
                EditorUtility.SetDirty(chart);
                e.Use();
                Repaint();
                return;
            }
        }
        //롱 노트 길이 축소
        if(e.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftArrow&&dragIndex>=0)
        {
            if (chart.notes[dragIndex].longNoteSize <= 0)
            {
                chart.notes[dragIndex].isLongNote = false;
                
                e.Use();
                Repaint();
                return;
            }
            chart.notes[dragIndex].longNoteSize-=1f/chart.snapDiv;
            e.Use();
            Repaint();
            return;
        }

        //롱 노트 길이 확대
        if(e.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightArrow&&dragIndex>=0)
        {
            if (!chart.notes[dragIndex].isLongNote)
            {
                chart.notes[dragIndex].isLongNote = true;
                chart.notes[dragIndex].longNoteSize+=1f/chart.snapDiv;
                e.Use();
                Repaint();
                return;
            }
            chart.notes[dragIndex].longNoteSize+=1f/chart.snapDiv;
            e.Use();
            Repaint();
            return;
            
        }

        if(e.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.A||Event.current.keyCode == KeyCode.B) && isPlaying)
        {
            Undo.RecordObject(chart, "Add Note");
            NoteType type = Event.current.keyCode == KeyCode.A ? NoteType.A : NoteType.B;
            float beat = SnapBeat(GetSongTimeSec());

            chart.notes.Add(new RhythmNote { beat = Mathf.Max(0f, beat), type = type });
            chart.notes = chart.notes.OrderBy(n => n.beat).ThenBy(n => n.type).ToList();

            EditorUtility.SetDirty(chart);
            e.Use();
            Repaint();
            return;
        }

        // 드래그 이동
        if (isDragging && dragIndex >= 0 && e.type == EventType.MouseDrag && e.button == 0)
        {
            Undo.RecordObject(chart, "Move Note");

            NoteType type = MouseToLane(rect, e.mousePosition.y);
            float beat = SnapBeat(MouseToBeat(rect, e.mousePosition.x));

            chart.notes[dragIndex].beat = Mathf.Max(0f, beat);
            chart.notes[dragIndex].type = type;

            EditorUtility.SetDirty(chart);
            e.Use();
            Repaint();
            return;
        }

        // 드래그 종료: 정렬
        if (isDragging && e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;

            Undo.RecordObject(chart, "Sort After Drag");
            var moved = chart.notes[dragIndex];
            chart.notes = chart.notes.OrderBy(n => n.beat).ThenBy(n => n.type).ToList();
            dragIndex = FindClosestIndex(chart.notes, moved);

            EditorUtility.SetDirty(chart);
            e.Use();
            Repaint();
            return;
        }
    }

    // ===================== 재생 로직 =====================
    private float GetSongTimeSec()
    {
        if (chart == null || chart.clip == null) return 0f;

        if (!isPlaying) return Mathf.Clamp(playOffsetSec, 0f, chart.clip.length);

        double now = EditorApplication.timeSinceStartup;
        float t = playOffsetSec + (float)(now - playStartEditorTime);
        return Mathf.Clamp(t, 0f, chart.clip.length);
    }

    private void Seek(float timeSec)
    {
        if (chart == null || chart.clip == null) return;

        timeSec = Mathf.Clamp(timeSec, 0f, chart.clip.length);
        playOffsetSec = timeSec;

        if (isPlaying)
        {
            PlayPreview(playOffsetSec); // 재생 중이면 해당 위치부터 재시작
        }
        else
        {
            Repaint();
        }
    }

    private void PlayPreview(float startTimeSec)
    {
        if (chart == null || chart.clip == null) return;

        StopAllPreview();

        isPlaying = true;
        playStartEditorTime = EditorApplication.timeSinceStartup;

        int startSample = Mathf.Clamp((int)(startTimeSec * chart.clip.frequency), 0, chart.clip.samples - 1);

        // AudioUtil.PlayPreviewClip(clip, startSample, loop=false)
        PlayPreviewClipMethod?.Invoke(null, new object[] { chart.clip, startSample, false });
    }

    private void PausePreview()
    {
        if (!isPlaying) return;

        playOffsetSec = GetSongTimeSec();
        isPlaying = false;
        StopAllPreview();
    }

    private void StopPreview()
    {
        isPlaying = false;
        playOffsetSec = 0f;
        StopAllPreview();
        Repaint();
    }

    private void StopAllPreview()
    {
        StopAllPreviewClipsMethod?.Invoke(null, null);
    }

    // ===================== 유틸 =====================
    
    private float SecToBeat(float sec){
        if (chart != null)
        {
            return sec / (60f / Mathf.Max(1f, chart.bpm));
        }
        return -1;
       
    }
    private float BeatToSec(float beat) => beat * (60f / Mathf.Max(1f, chart.bpm));

    private float MouseToBeat(Rect rect, float mouseX)
    {
        float ppb = pixelsPerBeat * zoom;
        float xLocal = (mouseX - rect.x) + scrollX;
        return xLocal / ppb;
    }

    private NoteType MouseToLane(Rect rect, float mouseY)
    {
        float y = mouseY - rect.y;
        return (y < LaneH) ? NoteType.A : NoteType.B;
    }

    private float SnapBeat(float beat)
    {
        int div = Mathf.Max(1, chart.snapDiv);
        return Mathf.Round(beat * div) / div;
    }

    private int PickNoteIndex(Rect rect, Vector2 mousePos)
    {
        float ppb = pixelsPerBeat * zoom;

        float width = 12f;
        const float pickRadiusY = 16f;

        for (int i = 0; i < chart.notes.Count; i++)
        {
            var n = chart.notes[i];
            float x = rect.x + (n.beat * ppb) - scrollX;

            if (n.isLongNote)
            {
                width = ppb * Mathf.Max(0.1f, n.longNoteSize);
            }


            float yCenter = (n.type == NoteType.A)
                ? rect.y + LaneH * 0.5f
                : rect.y + LaneH + LaneH * 0.5f;

            if (Mathf.Abs(mousePos.x - x) <= width &&
                Mathf.Abs(mousePos.y - yCenter) <= pickRadiusY)
                return i;
        }
        return -1;
    }

    private int FindClosestIndex(List<RhythmNote> list, RhythmNote target)
    {
        int best = -1;
        float bestScore = float.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            var n = list[i];
            if (n.type != target.type) continue;

            float score = Mathf.Abs(n.beat - target.beat);
            if (score < bestScore)
            {
                bestScore = score;
                best = i;
            }
        }
        return best;
    }

    // ===================== 리스트 =====================
    private void DrawNotesList()
    {
        EditorGUILayout.LabelField($"Notes ({chart.notes.Count})", EditorStyles.boldLabel);

        listScroll = EditorGUILayout.BeginScrollView(listScroll, GUILayout.Height(Mathf.Min(240, chart.notes.Count * 22 + 70)));

        for (int i = 0; i < chart.notes.Count; i++)
        {
            var n = chart.notes[i];
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"#{i}", GUILayout.Width(30));
            n.type = (NoteType)EditorGUILayout.EnumPopup(n.type, GUILayout.Width(50));
            n.beat = EditorGUILayout.FloatField(n.beat, GUILayout.Width(90));
            GUILayout.Label("LongNoteSetting", GUILayout.Width(120));
            n.isLongNote = EditorGUILayout.Toggle(n.isLongNote,GUILayout.Width(25));
            if(n.isLongNote) n.longNoteSize = EditorGUILayout.FloatField(n.longNoteSize,GUILayout.Width(75));

            if (GUILayout.Button("Seek", GUILayout.Width(50)))
                Seek(BeatToSec(n.beat));

            if (GUILayout.Button("X", GUILayout.Width(24)))
            {
                Undo.RecordObject(chart, "Remove Note");
                chart.notes.RemoveAt(i);
                EditorUtility.SetDirty(chart);
                
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.HelpBox(
            "타임라인 조작:\n- 좌클릭: 노트 추가 / 노트 선택\n- 드래그: 노트 이동(스냅 적용)\n- 우클릭: 노트 삭제\n- Ctrl+좌클릭: 재생 위치 이동(Seek)\n- 휠: 좌우 스크롤, Shift+휠: 줌\n-(실행하면서) A,B키: 현재 재생 시점에 노트를 찍음 \n노트 리스트:\n-#뒤에 오는 숫자는 노트의 인덱스입니다.\n-드롭박스는 노트가 몇번레인에 올지 정하는 메뉴이며, 1번레인이 A, 2번레인이 B입니다.\n-숫자는 몇번 박자에 노트가 올지 표기하는 숫자이며, 되도록 클릭 툴로 편집하는 것을 권장합니다.",
            MessageType.None);
    }
}
#endif
