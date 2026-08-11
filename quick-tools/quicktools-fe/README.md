# QuicktoolsFe

- https://optimus.openng.org/listbox

## Prompt

```txt
Trong angular,
Sử dụng PrimeNG Theme (styled mode) làm nguồn màu duy nhất cho toàn bộ giao diện.

Toàn bộ màu phải lấy từ PrimeNG CSS variables (--p-*).

Quy tắc sử dụng màu:

1. Background:
- Page background:
  var(--p-ground-background)

- Card, panel, container:
  var(--p-content-background)

- Hover background của component:
  var(--p-content-hover-background)

- Không sử dụng --p-surface-50, --p-surface-100 cho background của component vì chúng không đảm bảo đảo màu chính xác giữa light/dark mode.

2. Text:
- Text chính:
  var(--p-text-color)

- Text phụ, label, description:
  var(--p-text-muted-color)

- Text khi hover:
  var(--p-text-hover-color)

3. Border:
- Border mặc định:
  var(--p-content-border-color)

4. Primary / Brand color:
Sử dụng semantic primary token của PrimeNG:

- Main:
  var(--p-primary-color)

- Hover:
  var(--p-primary-hover-color)

- Active:
  var(--p-primary-active-color)

- Text/icon trên primary background:
  var(--p-primary-inverse-color)

5. Highlight / Selection:
Sử dụng:

- Background:
  var(--p-highlight-background)

- Focus background:
  var(--p-highlight-focus-background)

- Text:
  var(--p-highlight-color)

Không tự tạo màu hover bằng rgba hoặc hex.

6. Input/Form:
Sử dụng PrimeNG form field tokens:

- Background:
  var(--p-form-field-background)

- Text:
  var(--p-form-field-color)

- Border:
  var(--p-form-field-border-color)

- Placeholder:
  var(--p-form-field-placeholder-color)

- Focus:
  var(--p-primary-color)


Yêu cầu hỗ trợ Dark Mode:
- Giao diện phải tự động thích nghi khi thay đổi class .dark.
- Không hard-code màu light hoặc dark.
- Không dùng media query prefers-color-scheme.
- Không tạo riêng CSS cho dark mode.
- Chỉ thông qua PrimeNG semantic tokens.

Phong cách:
- Dùng CSS thuần hoặc CSS module/scoped CSS cho layout.
- PrimeNG quản lý toàn bộ màu sắc.
- Layout, spacing, typography có thể tự thiết kế nhưng tuyệt đối không thay thế hệ thống màu của PrimeNG.

Mục tiêu:
Một giao diện có thể đổi toàn bộ theme chỉ bằng cách thay đổi PrimeNG preset/semantic config mà không cần sửa HTML/CSS.

Đồng thời phải sử dụng các control hiện đại, ví dụ @for, @if

mô tả giao diện:

tạo 1 component cho navigation chứa service trên và 1 nút để quay lại trang trước đó

```