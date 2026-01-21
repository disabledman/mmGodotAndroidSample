using Godot;

public partial class CubeController : Node3D
{
	[Export]
	public NodePath CubePath { get; set; }

	[Export]
	public float RotationSpeed { get; set; } = 0.3f;

	private Node3D _cube;
	private bool _isDragging = false;
	private Vector2 _lastPos;

	[Export] public NodePath CameraPath;          // 在編輯器拖入你的 Camera3D 或 SpringArm3D
	private Camera3D _camera;

	private float zoomSensitivity = 15.0f;   // 調整這個值控制 pinch 靈敏度（越大越快）
	private float _zoomDistance = 5f;              // 初始距離
	private float _minZoom = 1.5f;
	private float _maxZoom = 20f;
	private float _zoomSpeed = 0.8f;

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
		
		_camera = GetNode<Camera3D>(CameraPath);
		
		ApplyZoom(0);
	}

	public override void _Input(InputEvent @event)
	{
		if (_cube == null)
			return;

		// ── 觸控部分保持不變 ───────────────────────────────
		if (@event is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Index == 0)
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
		}
		else if (@event is InputEventScreenDrag dragEvent)
		{
			if (dragEvent.Index == 0 && _isDragging)
			{
				HandleDrag(dragEvent.Position);
			}
		}
		// ── 新增：Pinch Zoom (兩指捏合) ───────────────────────────────
		else if (@event is InputEventMagnifyGesture magnifyEvent)
		{
			// factor > 1.0 = 手指分開 → zoom in (放大)
			// factor < 1.0 = 手指合攏 → zoom out (縮小)
			float zoomDelta = (magnifyEvent.Factor - 1.0f) * zoomSensitivity;

			ApplyZoom(zoomDelta);

			// 可選：避免單指拖曳跟 pinch 衝突
			_isDragging = false;
		}
	
		// ── 滑鼠按鍵 + 滾輪 ───────────────────────────────────
		else if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				_isDragging = mouseButton.Pressed;
				if (mouseButton.Pressed)
				{
					_lastPos = mouseButton.Position;
				}
			}

			// 這裡加入滾輪縮放（只在按下時觸發，Godot 滾輪事件只有 Pressed = true）
			else if (mouseButton.Pressed)
			{
				float zoomDelta = 0f;

				if (mouseButton.ButtonIndex == MouseButton.WheelUp)
				{
					zoomDelta = -0.5f;   // 向上滾 → 拉近（zoom in）
				}
				else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
				{
					zoomDelta = 0.5f;    // 向下滾 → 拉遠（zoom out）
				}

				if (zoomDelta != 0f)
				{
					ApplyZoom(zoomDelta);
				}
			}
		}

		// ── 滑鼠移動（拖曳旋轉） ───────────────────────────────
		else if (@event is InputEventMouseMotion mouseMotion)
		{
			if (_isDragging)
			{
				HandleDrag(mouseMotion.Position);
			}
		}
	}

	private void ApplyZoom(float delta)
	{
		_zoomDistance += delta * _zoomSpeed;
		_zoomDistance = Mathf.Clamp(_zoomDistance, _minZoom, _maxZoom);

		// 如果用 SpringArm3D（推薦 orbit 時用）
		// _springArm.SpringLength = _zoomDistance;

		// 如果直接移動 Camera3D（假設相機看著 _cube）
		if (_camera != null)
		{
			Vector3 direction = (_cube.GlobalPosition - _camera.GlobalPosition).Normalized();
			_camera.GlobalPosition = _cube.GlobalPosition - direction * _zoomDistance;
		}
	}

	private void HandleDrag(Vector2 currentPos)
	{
		if (!_isDragging)
		{
			_isDragging = true;
			_lastPos = currentPos;
			return;
		}

		Vector2 delta = currentPos - _lastPos;
		_lastPos = currentPos;

		// 將拖曳位移轉換成旋轉角度（弧度）
		float yaw = -delta.X * RotationSpeed * Mathf.DegToRad(1.0f);   // 水平：繞 Y 軸
		float pitch = -delta.Y * RotationSpeed * Mathf.DegToRad(1.0f); // 垂直：繞 X 軸

		_cube.RotateObjectLocal(Vector3.Up, yaw);
		_cube.RotateObjectLocal(Vector3.Right, pitch);
	}
}
