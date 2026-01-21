## Godot 4.6 Mono Android 正方體旋轉範例

# Snapshots

<img width="645" height="487" alt="image" src="https://github.com/user-attachments/assets/23d5d6f9-5b39-467d-9398-8fd096d39c43" />

<img width="858" height="1907" alt="image" src="https://github.com/user-attachments/assets/4a7713dc-53cc-4908-86ce-a3e622ab2cb9" />



這個專案提供使用 **Godot 4.6 .NET（Mono）** 開發 Android App 的說明與 C# 腳本範例。
畫面中會有：

- 一個 3D 正方體（Box）
- 一個環境光源（Environment + Directional/Omni Light）
- 一個攝影機（Camera3D）
- 透過手指（或滑鼠）拖曳，自由旋轉正方體（繞 X/Y 軸）
- PC 上使用滑鼠滾輪、Android 上使用雙指手勢來放大縮小正方體
- 支援 Android 直立／橫向自動切換

> 提示：你實際的 Godot 專案會建在本機某個資料夾中，本 repo 主要是紀錄步驟與提供 C# 腳本範例。

---

## 1. 安裝與設定 Godot 4.6 .NET（Mono）

1. 前往 Godot 官方下載頁，下載 **Godot 4.6 .NET（Mono）** 版本（Windows 版）。
2. 解壓縮或安裝後，啟動 `Godot_v4.6*.net.exe`（名稱依版本稍有不同）。
3. 在 Godot 中設定 .NET SDK：
   - 進入 `Editor > Editor Settings...`
   - 搜尋 `.NET` 或展開 `Mono` / `.NET` 區段
   - 設定你安裝的 **.NET SDK** 路徑（例如 .NET 8 SDK）
   - 重新啟動 Godot 確保可以建立 C# 腳本

4. 建立新的 3D 專案：
   - 在啟動畫面選擇 `New Project`
   - 專案名稱：`AndroidCubeRotate`
   - 專案資料夾：選擇任意空資料夾（與本 repo 可以不同）
   - Template 選擇 `3D`
   - 建立後開啟專案

---

## 2. 設定 Android 匯出環境

### 2.1 安裝 Android Export Templates

1. 在 Godot 中打開你的專案。
2. 進入 `Editor > Manage Export Templates...`
3. 安裝對應 **4.6** 版本的官方 Export Templates（可線上下載或手動載入）。

### 2.2 設定 Android SDK / JDK / adb

1. 在 Godot 中進入：`Editor > Editor Settings...`
2. 左側展開：`Export > Android`
3. 設定：
   - `Android SDK Path`：指向你的 Android SDK 目錄（需包含 `platform-tools`、`build-tools` 等）。
   - `JDK Path`：指向安裝的 JDK（通常建議使用 JDK 17，或依 Godot 官方建議）。
   - `adb`：通常在 `Android SDK/platform-tools/adb`，如有需要可明確指定。

### 2.3 建立 Android 匯出設定

1. 開啟 `Project > Export...`
2. 按 `Add...`，選擇 `Android`。
3. 在 `Options` 中設定：
   - `Package/Unique Name`：例如 `com.example.androidcuberotate`
   - `Version Code` / `Version Name`：例如 `1` / `1.0.0`
4. 在 `Screen` 或 `Orientation` 相關欄位：
   - 啟用 `Portrait` 與 `Landscape`，讓 Android 可自動直橫切換。

---

## 3. 建立場景與節點結構

### 3.1 主場景結構

在 Godot 中：

1. 新增一個場景（`Scene > New Scene`）。
2. 根節點選擇 `Node3D`，命名為 `Main`。
3. 在 `Main` 底下新增節點：
   - `MeshInstance3D`（命名為 `Cube`）
     - 在 Inspector 中的 `Mesh` 選擇 `BoxMesh` 作為正方體。
   - `DirectionalLight3D` 或 `OmniLight3D`（例如命名為 `MainLight`）
   - `WorldEnvironment`
     - 在 Inspector 的 `Environment` 建立一個新的 `Environment` 資源
     - 在 Environment 中設定簡單的 `Sky` 或 `Ambient Light`（可先使用預設）
   - `Camera3D`（命名為 `MainCamera`）
     - 將位置設在 Z 軸正方向，例如 `Transform > Position = (0, 0, 5)`
     - 面向原點（確保 `Look At` 指向 (0, 0, 0)）
     - `Fov` 約 70–75，`Near` 0.1，`Far` 100 即可

4. 使用 `Scene > Save As...`，將場景儲存為 `Main.tscn`。
5. 在 `Project > Project Settings... > General > Application > Run > Main Scene` 中，指定 `Main.tscn` 為啟動場景。

---

## 4. 新增 C# 腳本：手指拖曳自由旋轉正方體

### 4.1 建立腳本並掛載

1. 在場景的 `Main` 節點上按右鍵，選擇 `Attach Script...`。
2. 選擇語言 `C#`，類別名稱輸入 `CubeController`，檔案路徑可接受預設（例如 `Scripts/CubeController.cs`）。
3. 按 `Create`。

接著，將本 repo 中的 `CubeController.cs` 內容複製到你專案內對應的腳本檔案中（或直接在 Godot 內貼上）。

在 Godot Editor 內，確保：

- `Main` 節點已掛載 `CubeController` 腳本。
- 在 `Inspector` 中，腳本的 `CubePath`（匯出屬性）指向場景中的 `Cube` 節點。
- 可視需要調整 `RotationSpeed`（旋轉速度）、`ZoomSpeed`（縮放速度）、`MinScale` 與 `MaxScale`（縮放範圍限制）。

### 4.2 輸入操作說明

- 在 Android 裝置上：
  - 單指按住並拖曳畫面，即可讓正方體繞 X/Y 軸旋轉。
  - 雙指捏合（pinch）手勢可以放大縮小正方體。
- 在 PC 上測試：
  - 使用滑鼠左鍵按住並拖曳，同樣可以旋轉立方體。
  - 使用滑鼠滾輪向上/向下滾動，可以放大/縮小正方體。

---

## 5. 支援直橫向自動切換與手勢設定

### 5.1 啟用 Android 手勢支援（重要）

為了讓雙指縮放手勢在 Android 上正常工作，需要在專案設定中啟用手勢：

1. 開啟 `Project > Project Settings...`
2. 在左側搜尋或展開：`Input Devices > Pointing > Android`
3. 確認 `Enable Pan And Scale Gestures` 已啟用（勾選）
4. 這會啟用 `InputEventMagnifyGesture` 事件，讓雙指捏合縮放功能正常運作

### 5.2 Android 匯出設定

在 `Project > Export... > Android` 的設定中：

- 確認勾選（或允許）`Portrait` 與 `Landscape`。
- 不設定強制固定方向，讓 Android 根據裝置旋轉自動切換。

### 5.3 畫面伸展設定（可選）

在 `Project > Project Settings... > Display > Window`：

- `Stretch > Mode`：可選 `viewport`。
- `Stretch > Aspect`：可選 `keep` 或 `expand`。

對於單純顯示一個居中的 3D 立方體，通常預設設定已足夠；若你有 UI 元件，可視需要微調。

---

## 6. 測試與匯出到 Android

### 6.1 在 PC 上測試

1. 在 Godot Editor 中按右上角的「執行」按鈕（或 F5）。
2. 使用滑鼠左鍵拖曳，確認正方體可以順暢旋轉。
3. 視需要在 `CubeController` 中調整 `RotationSpeed` 以改變手感。

### 6.2 在 Android 裝置上測試

1. 開啟 Android 手機的「開發人員選項」與「USB 偵錯」。
2. 將手機以 USB 連接至電腦，確認 `adb devices` 可以看到裝置。
3. 在 Godot 中打開 `Project > Export...`，選擇 Android 匯出設定。
4. 按 `Export & Run`，選擇輸出位置，Godot 會自動安裝到連線中的裝置並啟動。
5. 在手機上實際用手指拖曳正方體，並旋轉手機確認直橫向自動切換正常。

### 6.3 匯出正式 APK / AAB（上架用，可選）

1. 在 Android 匯出設定中設定簽章金鑰（Keystore）與對應密碼。
2. 匯出為 `AAB` 或 `APK`。
3. 確認版本號與套件名稱是否符合上架平台（例如 Google Play）的要求。

---

## 7. 延伸與優化（可選）

- **材質與光影**：為立方體加上 `StandardMaterial3D`，調整顏色、金屬度、粗糙度，讓畫面更有質感。
- **環境效果**：在 `WorldEnvironment` 中加入 `Sky`、AO、Bloom 等。
- **旋轉限制**：若不希望立方體完全翻轉，可在更新旋轉前限制 `RotationDegrees` 的範圍。
- **慣性效果**：記錄最後一次拖曳的角速度，在 `_Process` 中逐漸衰減，實現拖曳後慣性旋轉的效果。

---

## 8. 範例腳本來源

本 repo 內的 `CubeController.cs` 是一個可直接用於 Godot 4.6 .NET 的腳本範例，你可以：

- 直接複製到你的 Godot 專案中使用。
- 或依需求調整旋轉速度、軸向、慣性等行為。

