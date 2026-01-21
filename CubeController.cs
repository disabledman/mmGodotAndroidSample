using Godot;

public partial class CubeController : Node3D
{
    [Export]
    public NodePath CubePath { get; set; }

    [Export]
    public float RotationSpeed { get; set; } = 0.3f;

    [Export]
    public float ZoomSpeed { get; set; } = 0.1f;

    [Export]
    public float MinScale { get; set; } = 0.5f;

    [Export]
    public float MaxScale { get; set; } = 3.0f;

    private Node3D _cube;
    private bool _isDragging = false;
    private Vector2 _lastPos;

    public override void _Ready()
    {
        if (CubePath != null && !CubePath.IsEmpty)
        {
            _cube = GetNode<Node3D>(CubePath);
        }
        else
        {
            // 若未指定路徑，嘗試自動尋找名為 "Cube" 的子節點
            _cube = GetNodeOrNull<Node3D>("Cube");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_cube == null)
            return;

        // 觸控（Android）
        if (@event is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                _isDragging = true;
                _lastPos = touchEvent.Position;
            }
            else
            {
                _isDragging = false;
            }
        }
        else if (@event is InputEventScreenDrag dragEvent)
        {
            HandleDrag(dragEvent.Position);
        }
        // 滑鼠（PC 測試用）
        else if (@event is InputEventMouseButton mouseButton)
        {
            // 先檢查滾輪事件（滾輪事件在按下時觸發）
            if (mouseButton.Pressed)
            {
                if (mouseButton.ButtonIndex == MouseButton.WheelUp)
                {
                    HandleZoom(1.0f + ZoomSpeed);
                    @event.SetHandled(); // 標記事件已處理
                }
                else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
                {
                    HandleZoom(1.0f - ZoomSpeed);
                    @event.SetHandled(); // 標記事件已處理
                }
            }
            // 再檢查左鍵
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                _isDragging = mouseButton.Pressed;
                if (mouseButton.Pressed)
                {
                    _lastPos = mouseButton.Position;
                }
            }
        }
        else if (@event is InputEventMouseMotion mouseMotion)
        {
            // 只有在按下左鍵時才處理滑鼠移動
            if (_isDragging)
            {
                HandleDrag(mouseMotion.Position);
            }
        }
        // 雙指縮放手勢（Android）
        else if (@event is InputEventMagnifyGesture magnifyEvent)
        {
            HandleZoom(magnifyEvent.Factor);
            @event.SetHandled(); // 標記事件已處理
        }
    }

    // 同時在 _UnhandledInput 中處理手勢事件，確保能接收到
    public override void _UnhandledInput(InputEvent @event)
    {
        if (_cube == null)
            return;

        // 雙指縮放手勢（Android）- 也在未處理事件中檢查
        if (@event is InputEventMagnifyGesture magnifyEvent)
        {
            HandleZoom(magnifyEvent.Factor);
            @event.SetHandled();
        }
    }

    private void HandleDrag(Vector2 currentPos)
    {
        // 只有在拖曳狀態下才處理
        if (!_isDragging)
            return;

        Vector2 delta = currentPos - _lastPos;
        _lastPos = currentPos;

        // 將拖曳位移轉換成旋轉角度（弧度）
        float yaw = -delta.X * RotationSpeed * Mathf.DegToRad(1.0f);   // 水平：繞 Y 軸
        float pitch = -delta.Y * RotationSpeed * Mathf.DegToRad(1.0f); // 垂直：繞 X 軸

        _cube.RotateObjectLocal(Vector3.Up, yaw);
        _cube.RotateObjectLocal(Vector3.Right, pitch);
    }

    private void HandleZoom(float factor)
    {
        if (_cube == null)
            return;

        Vector3 currentScale = _cube.Scale;
        Vector3 newScale = currentScale * factor;

        // 限制縮放範圍
        float avgScale = (newScale.X + newScale.Y + newScale.Z) / 3.0f;
        if (avgScale < MinScale)
        {
            float scaleFactor = MinScale / avgScale;
            newScale *= scaleFactor;
        }
        else if (avgScale > MaxScale)
        {
            float scaleFactor = MaxScale / avgScale;
            newScale *= scaleFactor;
        }

        _cube.Scale = newScale;
    }
}

