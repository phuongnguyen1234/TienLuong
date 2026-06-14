# Requirements

## 1. Yêu cầu cơ bản

- Hiển thị danh sách theo phân trang
- Tìm kiếm theo Mã/Tên thành phần lương
- Lọc nhanh theo đơn vị công tác, trạng thái
- Thêm mới từ danh mục hệ thống hoặc trực tiếp trên form chi tiết
- Nhân bản, sửa, xóa một hoặc nhiều bản ghi
- Ghim cột, tùy chỉnh độ rộng, ẩn/hiện cột
- Lọc nâng cao theo nhiều tiêu chí

---

## 2. Yêu cầu chi tiết

### 2.1 Màn hình danh sách

- Hiển thị bảng phân trang
- Tìm kiếm theo Mã, Tên
- Lọc nhanh theo Đơn vị công tác, Trạng thái

### 2.2 Màn hình chi tiết

#### Data Validation

- Not null validation
- Mã duy nhất
- Format số, tiền tệ
- **Circular Reference Check**: Không cho phép lưu công thức nếu xảy ra tham chiếu vòng giữa các thành phần.
- Check xem TPL định xóa/ngừng theo dõi có đang bị TPL cha nào đó sử dụng không

#### User Experience

- Phím Tab để chuyển giữa các input
- Auto focus vào input đầu tiên khi mở form
- Validate input khi bị blur
- Xử lý giao diện khi thay đổi trường Tính chất
- Component FormulaInput.vue cho ô nhập công thức (Giá trị, Định mức)
- Hỗ trợ gợi ý (Intellisense) các mã thành phần lương hiện có.

#### Components

- Modal thông báo (Xóa, Hủy, ...)
- Tooltip cho các button icon
- Toast message

### 2.3 Validation Rules

#### Required Fields

- Mã thành phần lương (TPL)
- Tên TPL
- Loại Thành Phần
- Tính chất

#### Unique Constraint

- Mã TPL: unique

#### Max Length

- Mã TPL: max 255 ký tự
- Tên TPL: max 255 ký tự

### 2.4 API Requirements

#### Database Tables

- `pa_salary_composition_system` (Danh mục hệ thống)
- `pa_salary_composition` (Danh sách TPL)
- `pa_organization` (Danh sách đơn vị công tác)
- `pa_grid_config` (Danh sách các cột của bảng) _(optional)_

#### Features

- Lọc nâng cao danh sách
- Ghim cột
- Tùy chỉnh độ rộng cột
- Ẩn/hiện cột trên danh sách
- Sửa nhanh trường khi xem form
