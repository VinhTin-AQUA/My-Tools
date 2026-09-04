# QuicktoolsFe

- https://optimus.openng.org/listbox

## Prompt

```txt
Trong angular,
Sử dụng PrimeNG Theme (styled mode) làm nguồn màu duy nhất cho toàn bộ giao diện.

Toàn bộ màu phải lấy từ PrimeNG CSS variables (--p-*).

Quy tắc sử dụng màu:

 primary: {
    color: '{green.700}',
    inverseColor: '#ffffff',
    hoverColor: '{green.800}',
    activeColor: '{green.900}',
},

danger: {
    color: '{red.600}',
    inverseColor: '#ffffff',
    hoverColor: '{red.700}',
    activeColor: '{red.800}',
},

highlight: {
    background: '{green.100}',
    focusBackground: '{green.200}',
    color: '{green.950}',
    focusColor: '#000000',
},

// Surface - nền đen tuyền cho light mode
surface: {
    background: '#000000',
    card: '#0a0a0a',
    border: '#1a1a1a',
    hover: '#1a1a1a',
},

// Text màu sáng trên nền đen
text: {
    color: '#e5e5e5',
    hoverColor: '#ffffff',
    mutedColor: '#a3a3a3',
},



đồng thời phải sử dụng tailwind class inline html về bố cục, kích thước, .... riêng với màu sắc phải sử dụng custom như đã liệt kê ở trên và code trong css riêng. Lưu ý cho cả giao diện mobile

Một giao diện có thể đổi toàn bộ theme chỉ bằng cách thay đổi PrimeNG preset/semantic config mà không cần sửa HTML/CSS.

Đồng thời phải sử dụng các control hiện đại, ví dụ @for, @if

mô tả giao diện:


```