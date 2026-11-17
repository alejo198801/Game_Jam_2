using System.Collections.Generic;
using GameJam2.Core;
using GameJam2.Flow;
using GameJam2.Puzzles;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class GameplayHUD : MonoBehaviour
{
    [Header("Referencias del juego")]
    [SerializeField] private CodeManager codeManager;
    [SerializeField] private ObjectPuzzleManager objectPuzzleManager;
    [SerializeField] private DropZone nest;
    [SerializeField] private SpiderChase spider;
    [SerializeField] private GameManager gameManager;

    [Header("Fuentes: arrastrar los archivos TTF u OTF")]
    [SerializeField] private Font bodyFont;
    [SerializeField] private Font titleFont;
    [SerializeField, Min(0.5f)] private float noticeDuration = 2.2f;

    private static readonly Color Red = new Color32(230, 49, 56, 255);
    private static readonly Color Light = new Color32(229, 225, 223, 255);
    private static readonly Color Green = new Color32(71, 190, 131, 255);

    private VisualElement layer;
    private VisualElement counter;
    private VisualElement noticeOverlay;
    private VisualElement keypadRoot;
    private Label objectiveSection;
    private Label objectiveText;
    private Label countText;
    private Label noticeTitle;
    private Label noticeSubtitle;
    private readonly Queue<Notice> pending = new Queue<Notice>();
    private float noticeRemaining;
    private bool noticeVisible;
    private bool connected;
    private int phase;

    private struct Notice
    {
        public string Title;
        public string Subtitle;
        public Color Color;
    }

    private void OnEnable()
    {
        BuildUI();
        ConnectEvents();
    }

    private void Start()
    {
        BuildUI();
        if (gameManager == null) gameManager = GameManager.Instance;

        if (codeManager == null || objectPuzzleManager == null || nest == null || spider == null)
            Debug.LogWarning("GameplayHUD: asigna Code Manager, Object Puzzle Manager, Nest y Spider.", this);
    }

    private void ConnectEvents()
    {
        if (connected) return;
        if (codeManager != null) codeManager.OnCodeCorrect.AddListener(FirstPuzzleSolved);
        if (objectPuzzleManager != null) objectPuzzleManager.OnPuzzleSolved.AddListener(NestCompleted);
        if (spider != null) spider.OnChaseStart.AddListener(ChaseStarted);
        connected = true;
    }

    private void BuildUI()
    {
        if (layer != null) return;
        VisualElement documentRoot = GetComponent<UIDocument>().rootVisualElement;
        if (documentRoot == null) return;
        keypadRoot = documentRoot.Q("puzzle-root");

        layer = new VisualElement { name = "gameplay-hud", pickingMode = PickingMode.Ignore };
        FullScreen(layer);
        if (bodyFont != null)
            layer.style.unityFontDefinition = new StyleFontDefinition(FontDefinition.FromFont(bodyFont));

        var objective = new VisualElement();
        objective.style.position = Position.Absolute;
        objective.style.left = 36;
        objective.style.top = 36;
        objective.style.width = 620;
        objective.style.backgroundColor = new Color(0.078f, 0.071f, 0.086f, 0.65f);
        objective.style.borderLeftWidth = 4;
        objective.style.borderLeftColor = Red;
        Padding(objective, 14);
        objectiveSection = MakeLabel("", 18, Red);
        objectiveText = MakeLabel("", 30, Light);
        objectiveText.style.whiteSpace = WhiteSpace.Normal;
        objective.Add(objectiveSection);
        objective.Add(objectiveText);
        layer.Add(objective);

        counter = new VisualElement();
        counter.style.position = Position.Absolute;
        counter.style.right = 36;
        counter.style.top = 36;
        counter.style.width = 240;
        counter.style.backgroundColor = new Color(0.078f, 0.071f, 0.086f, 0.8f);
        counter.style.borderTopWidth = counter.style.borderBottomWidth = 1;
        counter.style.borderLeftWidth = counter.style.borderRightWidth = 1;
        counter.style.borderTopColor = counter.style.borderBottomColor = Red;
        counter.style.borderLeftColor = counter.style.borderRightColor = Red;
        Padding(counter, 12);
        var countHeader = MakeLabel("HUEVOS EN EL NIDO", 18, Red);
        countHeader.style.unityTextAlign = TextAnchor.MiddleCenter;
        countText = MakeLabel("", 36, Light);
        countText.style.unityTextAlign = TextAnchor.MiddleCenter;
        counter.Add(countHeader);
        counter.Add(countText);
        layer.Add(counter);

        noticeOverlay = new VisualElement();
        FullScreen(noticeOverlay);
        noticeOverlay.style.justifyContent = Justify.Center;
        var banner = new VisualElement();
        banner.style.backgroundColor = new Color(0.02f, 0.02f, 0.02f, 0.75f);
        Padding(banner, 24);
        noticeTitle = MakeLabel("", 52, Green);
        if (titleFont != null)
            noticeTitle.style.unityFontDefinition = new StyleFontDefinition(FontDefinition.FromFont(titleFont));
        noticeTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        noticeTitle.style.whiteSpace = WhiteSpace.Normal;
        noticeSubtitle = MakeLabel("", 24, Light);
        noticeSubtitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        noticeSubtitle.style.whiteSpace = WhiteSpace.Normal;
        banner.Add(noticeTitle);
        banner.Add(noticeSubtitle);
        noticeOverlay.Add(banner);
        noticeOverlay.style.display = DisplayStyle.None;
        layer.Add(noticeOverlay);

        
        layer.Query<VisualElement>().ForEach(element => element.pickingMode = PickingMode.Ignore);
        documentRoot.Add(layer);
        RefreshObjective();
    }

    private static void FullScreen(VisualElement element)
    {
        element.style.position = Position.Absolute;
        element.style.left = element.style.right = 0;
        element.style.top = element.style.bottom = 0;
    }

    private static void Padding(VisualElement element, int value)
    {
        element.style.paddingLeft = element.style.paddingRight = value;
        element.style.paddingTop = element.style.paddingBottom = value;
    }

    private static Label MakeLabel(string text, int size, Color color)
    {
        var label = new Label(text) { pickingMode = PickingMode.Ignore };
        label.style.fontSize = size;
        label.style.color = color;
        label.style.marginLeft = label.style.marginRight = 0;
        label.style.marginTop = label.style.marginBottom = 0;
        return label;
    }

    public void FirstPuzzleSolved()
    {
        if (phase != 0) return;
        phase = 1;
        RefreshObjective();
        EnqueueNotice("PUERTA ABIERTA", "Busca los huevos y devuélvelos al nido.", Green);
    }

    private void NestCompleted()
    {
        if (phase >= 2) return;
        phase = 2;
        RefreshObjective();
        EnqueueNotice("NIDO COMPLETO", "La puerta del pasillo se abre...", Green);
    }

    private void ChaseStarted()
    {
        if (phase >= 3) return;
        phase = 3;
        RefreshObjective();
       
        EnqueueNotice("¡HUYE!", "Encuentra la salida.", Red);
    }

    private void RefreshObjective()
    {
        if (layer == null) return;
        objectiveSection.text = phase == 0 ? "PUZZLE 1" : phase == 1 ? "PUZZLE 2" : "ESCAPE";
        objectiveText.text = phase == 0 ? "BUSCA EL CÓDIGO EN LOS CUADROS" :
            phase == 1 ? "DEVUELVE LOS HUEVOS AL NIDO" : "ESCAPA DE LA ARAÑA";
        counter.style.display = phase > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void EnqueueNotice(string title, string subtitle, Color color)
    {
        pending.Enqueue(new Notice { Title = title, Subtitle = subtitle, Color = color });
    }

    private void LateUpdate()
    {
        if (layer == null) return;

        if (gameManager != null && gameManager.State != GameState.Playing)
        {
            layer.style.display = DisplayStyle.None;
            pending.Clear();
            return;
        }

        bool keypadOpen = keypadRoot != null && keypadRoot.resolvedStyle.display != DisplayStyle.None;
        bool visible = Time.timeScale > 0f && !PlayerControlLock.IsLocked && !keypadOpen;
        layer.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        if (nest != null)
            countText.text = nest.CorrectCount + " / " + nest.Capacity;

        if (!visible) return;

        
        if (noticeVisible)
        {
            noticeRemaining -= Time.deltaTime;
            if (noticeRemaining <= 0f)
            {
                noticeVisible = false;
                noticeOverlay.style.display = DisplayStyle.None;
            }
        }

        if (!noticeVisible && pending.Count > 0)
        {
            Notice next = pending.Dequeue();
            noticeTitle.text = next.Title;
            noticeTitle.style.color = next.Color;
            noticeSubtitle.text = next.Subtitle;
            noticeRemaining = noticeDuration;
            noticeVisible = true;
            noticeOverlay.style.display = DisplayStyle.Flex;
        }
    }

    private void OnDisable()
    {
        if (connected)
        {
            if (codeManager != null) codeManager.OnCodeCorrect.RemoveListener(FirstPuzzleSolved);
            if (objectPuzzleManager != null) objectPuzzleManager.OnPuzzleSolved.RemoveListener(NestCompleted);
            if (spider != null) spider.OnChaseStart.RemoveListener(ChaseStarted);
        }
        connected = false;
        if (layer != null) layer.RemoveFromHierarchy();
        layer = null;
        noticeVisible = false;
        pending.Clear();
    }
}
