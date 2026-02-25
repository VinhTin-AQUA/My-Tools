package com.newtun.billinsight

import android.os.Bundle
import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.WindowInsetsControllerCompat
import androidx.core.view.ViewCompat

class MainActivity : TauriActivity() {
  override fun onCreate(savedInstanceState: Bundle?) {

    super.onCreate(savedInstanceState)

    // Allow content to extend under the system bars
    WindowCompat.setDecorFitsSystemWindows(window, false)

    val windowInsetsController = WindowCompat.getInsetsController(window, window.decorView)

    windowInsetsController.apply {
        show(WindowInsetsCompat.Type.systemBars())  // luôn hiển thị
        systemBarsBehavior = WindowInsetsControllerCompat.BEHAVIOR_DEFAULT  // không ẩn tự động
    }

    window.statusBarColor = android.graphics.Color.RED  // ví dụ status bar màu đen
    window.navigationBarColor = android.graphics.Color.BLACK  // navigation bar màu đỏ


    if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.P) {
        window.attributes.layoutInDisplayCutoutMode =
            android.view.WindowManager.LayoutParams.LAYOUT_IN_DISPLAY_CUTOUT_MODE_NEVER
    }
  }
}
