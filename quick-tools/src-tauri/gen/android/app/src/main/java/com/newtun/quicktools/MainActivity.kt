package com.newtun.quicktools

import android.os.Bundle

import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.WindowInsetsControllerCompat
import androidx.core.view.ViewCompat

class MainActivity : TauriActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {

        super.onCreate(savedInstanceState)

        window.decorView.setBackgroundColor(
            resources.getColor(R.color.webview_background, theme)
        )
        window.setBackgroundDrawableResource(R.color.webview_background)

        // Allow content to extend under the system bars
        WindowCompat.setDecorFitsSystemWindows(window, false)

        // 1. Lấy windowInsetsController (vẫn cần để quản lý system bars)
        val windowInsetsController = WindowCompat.getInsetsController(window, window.decorView)

        // 2. Hiển thị cả status bar và navigation bar
        windowInsetsController.apply {
            show(WindowInsetsCompat.Type.systemBars())  // luôn hiển thị
            systemBarsBehavior = WindowInsetsControllerCompat.BEHAVIOR_DEFAULT  // không ẩn tự động
        }

        // 3. Thiết lập màu sắc
        window.statusBarColor = android.graphics.Color.RED  // ví dụ status bar màu đen
        window.navigationBarColor = android.graphics.Color.BLACK  // navigation bar màu đỏ

        // 4. Nội dung không che thanh hệ thống
        // Không cần thay đổi layoutInDisplayCutoutMode nếu bạn không muốn trải ra notch
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.P) {
            window.attributes.layoutInDisplayCutoutMode =
                android.view.WindowManager.LayoutParams.LAYOUT_IN_DISPLAY_CUTOUT_MODE_NEVER
        }
    }
}
