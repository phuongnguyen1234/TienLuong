# Component Redesign Guidelines

## 1. Color

### Primary Colors

- Primary color: `#34B057`
- Background: `#F2F2F2`

### Text Colors

- Màu chính: `#212121`
- Màu phụ: `#666666`
- Màu chữ placeholder: `#9E9E9E`

### DataGrid/Table Colors

- Border: `#E0E0E0`
- Header: `#f6f6f6`
- Hover 1 dòng: `#cdeadf`

### Status Colors

- Focus: `#eaf7ee`
- Error: `#e54848`
- Warning: `#ffdd00`
- Success/In use/Online: `#34B057`
- Waiting/Not in use: `#ff9900`

---

## 2. Icon

- Đa số sử dụng **20x20**, 1 số trường hợp **24x24**
- Các icon có các trạng thái: hover, press, tooltip

### Icon States

- **Normal**: border `#707070`
- **Hover**: border `#707070`, background `#f2f2f2`, tổng size là **36x36**
- **Press**: border `#707070`, background `#ebebeb`, tổng size là **36x36**

---

## 3. Button

### Types

- Primary, secondary, link

### Features

- Có thể thêm nút dropdown bên phải
- Icon có thể đặt bên trái label button

### States

- **Disable**: opacity 60%
- **Hover**: `#02b936` (riêng link thì có thêm underline)
- **Pressed**: `#198f3b`

### Style & Spacing

- Secondary button: chỉ có outline, màu hover/pressed áp dụng với viền và label/icon
- min-width: `80px`
- height: `32px`
- padding-left/right: `12px`
- border-radius: `8px`
- width: auto

### With Icon

- Icon dropdown: **20x20**, align center, nút dropdown width `40px`
- Nút với icon: icon **20x20**, padding left `12px`, gap `4px`, padding right `16px`

---

## 4. Input

- height: `32px`
- gap `8px` với thông báo validate dữ liệu
- InputTag ở disable state: tag có opacity 56%

---

## 5. Dropdown

- Nội dung scroll (cả scrollbar) padding: `8px` so với container

---

## 6. Toast

- Vị trí: top center
- height: `40px`
- Phần icon width: `40px`
- Phần text padding: left/right `12px`
- Icon x cách right: `8px`

---

## 7. Tooltip

- Dạng: bubble speech
- Mũi tên cách: `8px`
- Nội dung padding top: `12px`
- Hướng hiển thị: right, top, down

---

## 8. Scrollbar

- height: `6px`
- Cách góc: `4px`

### Scrollbar States

- **Normal**: `#9e9e9e`
- **Hover**: `#757575`
- **Press**: `#666666`

---

## 9. TableSettings

### Layout

- Vị trí: right so với trang
- Top cách button setting: 1 khoảng

### Content

- Title: Tùy chỉnh cột (bên cạnh có nút icon "Refresh" Lấy lại mặc định)
- Thanh tìm kiếm
- Các row: checkbox, tên cột, grip icon xuất hiện khi hover

### Footer

- Nút Lưu
