# WebUI

## Download

- download binary

```txt
https://github.com/webui-dev/webui/releases/tag/nightly

webui-linux-gcc-x64.zip

webui-windows-msvc-x64.zip
```

- đổi tên file

```txt
QuickTools/Native/webui/linux-x64/libwebui-2.so thành QuickTools/Native/webui/linux-x64/libwebui.so

QuickTools/Native/webui/win-x64/webui-2.dll thành QuickTools/Native/webui/win-x64/webui.dll

nếu dùng secure thì đổi file secure
QuickTools/Native/webui/win-x64/webui-2-secure.dll thành QuickTools/Native/webui/win-x64/webui.dll
```

## Code Invoke C++ webUI

```txt
QuickTools\Modules\WebUI
```

## JS call BE

- Tạo các Invoke native

```txt
QuickTools\Modules\WebUI
```

- Tạo interfaceBinder

```txt
QuickTools\Modules\WebUI\InterfaceBinder.cs
```

- BE setup config async, tạo các hàm, Bind các hàm

```cs
private static void Main(string[] args)
{
    NativeLibraryManager.Initialize();

    // Tạo window
    var window = WindowManagementMethods.webui_new_window();

    InterfaceBinder.Bind(window, "longTask", LongTaskHandler);
    InterfaceBinder.Bind(window, "getData", GetDataHandler);
    InterfaceBinder.Bind(window, "sendData", SendDataHandler);
    InterfaceBinder.Bind(window, "requestData", RequestDataHandler);
    InterfaceBinder.BindAsyncFunction(window, "asyncFunction", MyAsyncFunction);

    // Cấu hình async
    ConfigMethods.webui_set_config(webui_config.asynchronous_response, true);
    ConfigMethods.webui_set_event_blocking(window, false);

    // Show window
    WebUIManager.Show(window, "/wwwroot/index.html");


    // Wait
    WindowManagementMethods.webui_wait();

    // Cleanup
    WebUIManager.Cleanup();
}
```

### Setup FE

- Cấu trúc thư mục

```txt
QuickTools/
└── wwwroot/
    ├── index.html
    └── webui.js
```

- Lưu ý phải có file webui.js và import vào header html

```html
<!doctype html>
<html lang="en">
	<head>
		<meta charset="UTF-8" />
		<meta name="viewport" content="width=device-width, initial-scale=1.0" />
		<title>Document</title>

		<script src="webui.js"></script>

		...
	</head>
</html>
```

- FE Call các hàm, gửi và nhận dữ liệu

```html
<!doctype html>
<html lang="en">
	<head>
		<meta charset="UTF-8" />
		<meta name="viewport" content="width=device-width, initial-scale=1.0" />
		<title>Document</title>

		<script src="webui.js"></script>

		<style>
			body {
				font-family: Arial;
				padding: 20px;
				background: #f0f0f0;
			}
			.container {
				max-width: 800px;
				margin: 0 auto;
				background: white;
				padding: 20px;
				border-radius: 10px;
			}
			button {
				padding: 10px 20px;
				margin: 5px;
				border: none;
				border-radius: 5px;
				cursor: pointer;
			}
			.btn-primary {
				background: #007bff;
				color: white;
			}
			.btn-success {
				background: #28a745;
				color: white;
			}
			.btn-warning {
				background: #ffc107;
				color: black;
			}
			.btn-danger {
				background: #dc3545;
				color: white;
			}
			#result {
				margin-top: 20px;
				padding: 15px;
				background: #f8f9fa;
				border-radius: 5px;
				min-height: 50px;
			}
			.loading {
				color: #007bff;
				font-weight: bold;
			}
		</style>
	</head>
	<body>
		<div class="container">
			<h1>WebUI Async Examples</h1>

			<div>
				<h3>1. Non-blocking Long Task</h3>
				<button class="btn-primary" onclick="runLongTask()">Run 5s Task</button>
				<span id="status1"></span>
			</div>

			<div>
				<h3>2. Fetch Data</h3>
				<button class="btn-success" onclick="fetchData()">Fetch Data</button>
				<div id="dataResult"></div>
			</div>

			<div>
				<h3>3. Send Data</h3>
				<button class="btn-warning" onclick="sendData()">Send Data</button>
				<div id="sendResult"></div>
			</div>

			<div>
				<h3>4. Request-Response</h3>
				<button class="btn-danger" onclick="requestResponse()">Request Response</button>
				<div id="responseResult"></div>
			</div>

			<div id="result">Ready...</div>

			<button onclick="runAsync()">Run Async</button>
			<div id="result"></div>
		</div>

		<script>
			// 1. Non-blocking Long Task
			async function runLongTask() {
				document.getElementById('status1').textContent = ' Running...';
				document.getElementById('status1').className = 'loading';
				const result = await webui.call('longTask', 42);
				document.getElementById('status1').textContent = ' Done: ' + result;
				document.getElementById('status1').className = '';
			}

			// 2. Fetch Data
			async function fetchData() {
				document.getElementById('dataResult').textContent = 'Loading...';
				const data = await webui.call('getData');
				document.getElementById('dataResult').textContent = JSON.stringify(data, null, 2);
			}

			// 3. Send Data
			async function sendData() {
				const data = {
					name: 'Test User',
					email: 'test@example.com',
					message: 'Hello from browser!',
				};
				document.getElementById('sendResult').textContent = 'Sending...';
				const result = await webui.call('sendData', JSON.stringify(data));
				document.getElementById('sendResult').textContent = result;
			}

			// 4. Request-Response
			async function requestResponse() {
				const requestId = Date.now().toString();
				document.getElementById('responseResult').textContent = 'Requesting...';
				const result = await webui.call('requestData', requestId);
				document.getElementById('responseResult').textContent = result;
			}

			async function runAsync() {
				const result = await webui.call('asyncFunction');
				document.getElementById('result').textContent = JSON.stringify(result);
			}
		</script>
	</body>
</html>
```

- upload file

```cs

InterfaceBinder.BindAsyncFunction(window, "uploadFile", UploadFileAsync);

private static async Task<object> UploadFileAsync(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
{
    // Lấy dữ liệu từ JavaScript
    IntPtr dataPtr = InterfaceMethods.webui_interface_get_string_at(window, event_number, UIntPtr.Zero);
    string jsonData = Marshal.PtrToStringAnsi(dataPtr);

    Console.WriteLine($"[UploadFile] Received data length: {jsonData?.Length ?? 0}");

    // Parse JSON
    using var doc = JsonDocument.Parse(jsonData);
    var root = doc.RootElement;

    string fileName = root.GetProperty("fileName").GetString();
    string fileContent = root.GetProperty("fileContent").GetString();

    Console.WriteLine($"[UploadFile] File name: {fileName}");

    // Tạo thư mục uploads
    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    if (!Directory.Exists(uploadDir))
    {
        Directory.CreateDirectory(uploadDir);
    }

    // Decode Base64 thành byte array
    byte[] fileBytes = Convert.FromBase64String(fileContent);

    // Lưu file
    string safeFileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(fileName)}";
    string filePath = Path.Combine(uploadDir, safeFileName);

    await File.WriteAllBytesAsync(filePath, fileBytes);

    Console.WriteLine($"[UploadFile] File saved: {filePath} ({fileBytes.Length} bytes)");

    // Trả về kết quả
    return new
    {
        status = "success",
        fileName = safeFileName,
        filePath = filePath,
        size = fileBytes.Length,
        message = "File uploaded successfully!"
    };
}
```

```html
<!doctype html>
<html lang="en">
	<head>
		<meta charset="UTF-8" />
		<meta name="viewport" content="width=device-width, initial-scale=1.0" />
		<title>Document</title>
		<script src="webui.js"></script>
	</head>
	<body>
		<div>
			<!-- Drop Zone -->
			<div class="drop-zone" id="dropZone">
				<input type="file" id="fileInput" />
			</div>

			<!-- Upload Button -->
			<button id="uploadBtn" onclick="uploadFile()">Upload File</button>
		</div>

		<script>
			let selectedFile = null;

			document.getElementById('fileInput').addEventListener('change', function () {
				if (this.files.length > 0) {
					selectFile(this.files[0]);
				}
			});

			// ==================== Select File ====================
			function selectFile(file) {
				selectedFile = file;
			}

			// ==================== Upload ====================
			async function uploadFile() {
				if (!selectedFile) {
					alert('Please select a file first!');
					return;
				}

				console.log(selectedFile);

				try {
					// Đọc file thành Base64
					const base64 = await readFileAsBase64(selectedFile);

					// console.log(base64);

					// Gọi C# function
					const data = {
						fileName: selectedFile.name,
						fileContent: base64,
					};

					const response = await webui.call('uploadFile', JSON.stringify(data));

					const result = JSON.parse(response);

					console.log(result);
				} catch (error) {
					console.error(error);
				}
			}

			// ==================== Helper ====================
			function readFileAsBase64(file) {
				return new Promise((resolve, reject) => {
					const reader = new FileReader();
					reader.onload = e => {
						const base64 = e.target.result.split(',')[1];
						resolve(base64);
					};
					reader.onerror = reject;
					reader.readAsDataURL(file);
				});
			}

			function formatFileSize(bytes) {
				if (bytes === 0) return '0 Bytes';
				const k = 1024;
				const sizes = ['Bytes', 'KB', 'MB', 'GB'];
				const i = Math.floor(Math.log(bytes) / Math.log(k));
				return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
			}
		</script>
	</body>
</html>
```

### Setup Angular

- Các hàm BE giữ nguyên
- Điều chỉ lại cấu hình, mặc định webui_show native cỉ hiển thị những gì có trong html, không thể load js hay css, dẫn đến không hiển thị gì từ build angular, vì build angular ra file html,css, js. Cần cấu hình để webui load html, css, js

```cs
// Set root folder (thư mục chứa file HTML và các assets)
string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
FileAndFolderMethods.webui_set_root_folder(window, rootPath);
Console.WriteLine($"Root folder set to: {rootPath}");

// Show window
// WebUIManager.Show(window, "/wwwroot/index.html");
WebUIManager.Show(window, "index.html");
```

- Định nghĩa webUI runtime

```ts
declare var webui: any;
```

- trong index.html

```html
<head>
	<script src="./webui.js"></script>
</head>
```

- Service

```ts
import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root',
})
export class WebuiService {
	constructor() {}

	/**
	 * Gọi một hàm WebUI
	 * @param functionName Tên function đã bind trong C#
	 * @param args Các tham số truyền vào
	 * @returns Promise với kết quả từ C#
	 */
	call<T = any>(functionName: string, ...args: any[]): Promise<T> {
		return new Promise((resolve, reject) => {
			try {
				// Gọi webui.call với function name và các tham số
				// Nếu có nhiều tham số, truyền vào dạng array
				const result = webui.call(functionName, ...args);
				resolve(result);
			} catch (error) {
				reject(error);
			}
		});
	}

	/**
	 * Gọi hàm với tham số JSON
	 */
	callJson<T = any>(functionName: string, data: any): Promise<T> {
		return this.call<T>(functionName, JSON.stringify(data));
	}

	/**
	 * Upload file với chunk
	 */
	async uploadFileChunked(file: File, chunkSize: number = 1024 * 1024): Promise<any> {
		const totalChunks = Math.ceil(file.size / chunkSize);

		// 1. Start session
		const startResult = await this.callJson('startUpload', {
			fileName: file.name,
			totalChunks: totalChunks,
		});
		const sessionId = startResult.sessionId;

		// 2. Upload từng chunk
		for (let i = 0; i < totalChunks; i++) {
			const start = i * chunkSize;
			const end = Math.min(start + chunkSize, file.size);
			const chunk = file.slice(start, end);

			const chunkBase64 = await this.readChunkAsBase64(chunk);

			await this.callJson('uploadChunk', {
				sessionId: sessionId,
				chunkIndex: i,
				chunkData: chunkBase64,
				totalChunks: totalChunks,
			});
		}

		// 3. Finish upload
		const finishResult = await this.callJson('finishUpload', {
			sessionId: sessionId,
		});

		return finishResult;
	}

	/**
	 * Đọc chunk thành Base64
	 */
	private readChunkAsBase64(chunk: Blob): Promise<string> {
		return new Promise((resolve, reject) => {
			const reader = new FileReader();
			reader.onload = (e: any) => {
				const base64 = e.target.result.split(',')[1];
				resolve(base64);
			};
			reader.onerror = reject;
			reader.readAsDataURL(chunk);
		});
	}
}
```

- sử dụng trong component

```ts
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ButtonModule } from '@openng/optimus-ui/button';
import { WebuiService } from './webui-service';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, ButtonModule],
    templateUrl: './app.html',
    styleUrl: './app.css',
})
export class App {
    protected readonly title = signal('quicktools-fe');

    constructor(private webuiService: WebuiService) {}

    async longTask() {
        const r = await this.webuiService.call<string>('longTask', 2);
        console.log(r);
    }

    async getData() {
        const r = await this.webuiService.call<any>('getData');
        console.log(r);
    }

    async sendData() {
        const r = await this.webuiService.callJson<string>('sendData', {
            name: 'Pootin',
            age: 10,
        });
        console.log(r);
    }

    async asyncFunction() {
        const r = await this.webuiService.call<string>('asyncFunction');
        console.log(r);
    }
}
```
