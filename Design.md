# System Design

## 1. Database

### 1.1 Convention Chung

#### Column Type Conventions

- **character set**: `utf8mb4` và collation `utf8mb4_0900_as_ci`
- **ID columns**: Kiểu GUID/char(36) default UUID()
- **description**: max 500 ký tự
- **code/alias**: unique varchar
- **name**: max 100/255 ký tự (tùy bảng)
- **currency**: decimal(22,4), not null default 0
- **ratio/coefficient**: decimal(18, 4) not null default 0
- **numeric fields**: not null default 0
- **text/varchar**: default empty string
- **boolean**: tinyint (naming: is*, allow*, has\*, ...)

#### Relationships & Constraints

- **Master-Detail**: setup Cascade delete, không đặt Cascade update
- **Catalog-Usage**: setup one-to-many với Cascade no action
- **Foreign Keys**: luôn đánh index cho FK, composite index cho cột query nhiều
- **Comments**: có comment giải thích cho tên bảng và các cột
- **Enum fields**: comment rõ miền giá trị (ví dụ: 0: Nam, 1: Nữ)

#### Naming Conventions

- **Index**: `ix_table_column`
- **Foreign Key**: `fk_thistable_referencedtable`
- **Procedure**: `proc_action_table`
- **Stored Procedures**:
- ưu tiên sử dụng thay cho backend với các nghiệp vụ cơ bản như CRUD
- Một số procedure cân nhắc sử dụng: như proc_CloneSystemSalaryComposition (đưa TPL hệ thống vào sử dụng), proc_CheckSalaryCompositionUsage (Quét tất cả các bảng để check xem cấu hình có đang bị dùng không), proc_GetSalaryCompositionPaging (Phân trang + Lọc đệ quy phòng ban + Lọc dynamic filter nâng cao), proc_BulkActionSalaryComposition (Xóa hoặc Ngừng theo dõi hàng loạt theo danh sách ID truyền vào)

---

### 1.2 Bảng dữ liệu

#### pa_salary_composition_system (Danh mục hệ thống)

| Column                      | Type         | Notes                                                                                    |
| --------------------------- | ------------ | ---------------------------------------------------------------------------------------- |
| scs_id                      | GUID         | PK                                                                                       |
| scs_code                    | varchar(255) | Unique                                                                                   |
| scs_name                    | varchar(255) |                                                                                          |
| scs_type                    | int          | 0: Thông tin nhân viên, 1: Chấm công, 2: Doanh số, 3: KPI, 4: Sản phẩm, 5: Lương,        |
|                             |              | 6: Thuế TNCN, 7: Bảo hiểm - Công đoàn 8: Khác                                            |
| scs_nature                  | int          | 0: Thu nhập, 1: Khấu trừ, 2: Khác                                                        |
| scs_tax_status              | int          | 0: Không xác định/chịu thuế, 1: Chịu thuế, 2: Miễn thuế toàn phần, 3: Miễn thuế một phần |
| scs_is_tax_deductible       | tinyint      |                                                                                          |
| scs_taxable_expression      | text         | Công thức tính phần chịu thuế (cho Miễn thuế 1 phần)                                     |
| scs_exempt_expression       | text         | Công thức tính phần miễn thuế (cho Miễn thuế 1 phần)                                     |
| sc_limit_expression         | text         |                                                                                          |
| scs_value_type              | int          | 0: Tiền tệ, 1: Số, 2: Chữ, 3: Ngày, 4: Phần trăm                                         |
| scs_description             | varchar(500) |                                                                                          |
| scs_is_displayed_on_payroll | int          | 0: Không, 1: Có, 2: Chỉ hiển thị nếu khác 0                                              |
| scs_is_deleted              | tinyint      |                                                                                          |

#### pa_salary_composition (Danh sách thành phần lương)

| Column                     | Type         | Notes                                                                                    |
| -------------------------- | ------------ | ---------------------------------------------------------------------------------------- |
| sc_id                      | GUID         | PK                                                                                       |
| sc_code                    | varchar(255) | Unique                                                                                   |
| sc_name                    | varchar(255) |                                                                                          |
| sc_type                    | int          | 0: Thông tin nhân viên, 1: Chấm công, 2: Doanh số, 3: KPI, 4: Sản phẩm, 5: Lương,        |
|                            |              | 6: Thuế TNCN, 7: Bảo hiểm - Công đoàn 8: Khác                                            |
| sc_nature                  | int          | 0: Thu nhập, 1: Khấu trừ, 2: Khác                                                        |
| sc_tax_status              | int          | 0: Không xác định/chịu thuế, 1: Chịu thuế, 2: Miễn thuế toàn phần, 3: Miễn thuế một phần |
| sc_is_tax_deductible       | tinyint      |                                                                                          |
| sc_taxable_expression      | text         | Công thức tính phần chịu thuế (cho Miễn thuế 1 phần)                                     |
| sc_exempt_expression       | text         | Công thức tính phần miễn thuế (cho Miễn thuế 1 phần)                                     |
| sc_limit_expression        | text         |                                                                                          |
| sc_allow_exceed_limit      | tinyint      |                                                                                          |
| sc_value_type              | int          | 0: Tiền tệ, 1: Số, 2: Chữ, 3: Ngày, 4: Phần trăm                                         |
| sc_calculation_method      | int          | 0: Tự động tính tổng, 1: Tính theo công thức                                             |
| sc_aggregation_scope       | int          | 0: Cùng đơn vị, 1: Dưới quyền, 2: Thuộc cơ cấu                                           |
| sc_composition_id          | GUID         | FK -> pa_salary_composition (nullable)                                                   |
| sc_organization_level      | int          | 1-4 (nullable)                                                                           |
| sc_formula_expression      | text         | Công thức tính giá trị (Sử dụng khi sc_calculation_method = 1)                           |
| sc_description             | varchar(500) |                                                                                          |
| sc_is_displayed_on_payroll | int          | 0: Không, 1: Có, 2: Chỉ hiển thị nếu khác 0                                              |
| sc_source                  | int          | 0: Mặc định (Copy từ hệ thống), 1: Tự thêm                                               |
| sc_is_deleted              | tinyint      |                                                                                          |
| sc_status                  | int          | 0: Đang theo dõi, 1: Ngừng theo dõi                                                      |

#### Logic Nghiệp vụ Đặc thù

- **Đưa TPL hệ thống vào sử dụng**: Thực hiện sao chép (Deep Copy) toàn bộ thông tin từ `pa_salary_composition_system` sang `pa_salary_composition`.
- **Phân biệt**: Sau khi copy, bản ghi mới sẽ có `sc_source = 0`. Các bản ghi người dùng tự tạo mới hoàn toàn sẽ có `sc_source = 1`.
- **Ràng buộc**: Không cho phép copy nếu `sc_code` của TPL hệ thống đã tồn tại trong danh sách TPL của người dùng.

#### pa_organization (Đơn vị công tác)

| Column                 | Type         | Notes                           |
| ---------------------- | ------------ | ------------------------------- |
| organization_id        | GUID         | PK                              |
| organization_name      | varchar(255) |                                 |
| organization_parent_id | GUID         | FK -> pa_organization, nullable |

#### salarycomposition_organization (Bảng ánh xạ TPL - Đơn vị)

| Column          | Type | Notes                       |
| --------------- | ---- | --------------------------- |
| sc_id           | GUID | FK -> pa_salary_composition |
| organization_id | GUID | FK -> pa_organization       |

### 1.3 Stored Procedures

#### Bắt buộc

- **proc_insert_pa_salary_composition**: Thêm mới TPL
- **proc_update_pa_salary_composition**: Cập nhật thông tin TPL
- **proc_delete_pa_salary_composition**: Xóa logic (Soft delete)
- **proc_get_pa_salary_composition_by_id**: Lấy chi tiết 1 bản ghi
- **proc_get_pa_salary_composition_paging**: Lấy danh sách kèm phân trang, tìm kiếm và lọc đơn vị
- **proc_clone_system_salary_composition**: Copy TPL từ hệ thống vào danh sách sử dụng
- **proc_upsert_salarycomposition_organization**: Cập nhật danh sách đơn vị áp dụng cho TPL
- **proc_get_pa_organization**: Lấy danh mục đơn vị
- **proc_get_pa_salary_composition_system**: Lấy danh sách TPL mẫu hệ thống
- **proc_check_salary_composition_usage**: Kiểm tra TPL có đang được dùng trong công thức khác không

---

## 2. Backend (ASP.NET)

### 2.1 Convention Chung

- **Documentation**: Tất cả class viết doc với: Chức năng class, Created by, Ngày tạo
- **Method Documentation**: doc đầy đủ giải thích chức năng, comment các bước chính
- **Entity Documentation**: doc rõ ý nghĩa các cột
- **Connection Pooling**: sử dụng connection pooling
- **Caching**: IMemoryCache lưu trong vòng 5 phút
- **Race Condition**: xử lý race condition khi create trường Unique (code) từ tầng DB

---

### 2.2 Kiến trúc tổng quát

```
Backend/
├── Api/
│   ├── Controllers/
│   ├── Program.cs
│   └── Properties/
├── Core/
│   ├── DTO/
│   ├── Entities/
│   ├── Exceptions/
│   ├── Interfaces/
│   ├── Mappers/
│   ├── Middlewares/
│   ├── Services/
│   ├── Validators/
│   └── ServiceExtension.cs
└── Infrastructure/
    ├── Database/
    ├── Extensions/
    ├── Repository/
    ├── Queries/
    └── RepositoryExtension.cs
```

#### Api Layer

- **BaseController**: CRUD entity và query danh sách
- **Entity Controllers**: các class controller entity cụ thể

#### Core Layer

##### DTO

- FilterCriteria, FilterQueryParser, PagedResult, Result
- Các entity DTO

##### Entities

- **Inheritance**: BaseSalaryComposition (base), SalaryComposition & SalaryCompositionSystem (kế thừa)
- Các class Type, Organization

##### Exceptions

- GlobalExceptionMiddleware
- ValidationException (bắt buộc)
- Các exception khác

##### Interfaces

- **Database**: IBdConnectionFactory
- **Repository**: IBaseRepository<T>, IEntityRepository (kế thừa)
- **Service**: IBaseCrudService<TDto>, IEntityService (kế thừa)

##### Mappers

Mỗi mapper chứa 3 method:

- `static Entity ToEntity(DTO)`
- `void UpdateFromDto(Entity, DTO)`
- `static DTO ToResponseDTO(Entity)`

##### Services

- **BaseService<T>**
- **CrudService<TEntity, TDto, TRepository>** : BaseService<TEntity>, IBaseCrudService<TDto>
- **EntityService** : CrudService<>, IEntityService
- **SalaryCompositionService** : BaseService<SalaryComposition>, ISalaryCompositionService (nếu CRUD đặc thù)

##### Validators

- EntityValidator : AbstractValidator<DTO> (FluentValidator)

#### Infrastructure Layer

##### Repository

- **BaseRepository** : IBaseRepository<T>
- **EntityRepository** : BaseRepository<Entity>, IEntityRepository

##### Queries

- EntityQuery.json files cho từng entity

##### Database

- ConnectionFactory
- DatabaseOption
- StringExtension (snake_case → PascalCase)

---

## 3. Frontend (Vue.js)

### 3.1 Convention Chung

- **Toast Notification**: Mọi thao tác thành công/thất bại cần có thông báo Toast

---

### 3.2 Kiến trúc tổng quát

```
Frontend/
├── public/
├── src/
│   ├── assets/
│   │   ├── icons/
│   │   └── styles/
│   ├── components/
│   │   ├── base/
│   │   ├── controls/
│   │   ├── drawer/
│   │   ├── form/
│   │   ├── icons/
│   │   └── others/
│   ├── layouts/
│   ├── models/
│   ├── router/
│   ├── services/
│   ├── utils/
│   ├── views/
│   ├── App.vue
│   └── main.js
├── index.html
├── jsconfig.json
├── package.json
├── README.md
└── vite.config.js
```

---

### 3.3 Folder Structure Chi tiết

#### 3.3.1 Assets

##### Icons

- Chứa các tài nguyên icon tĩnh

##### Styles

- **style.css**: nhúng các file css còn lại
- **variables.css**: z-index system, color, gap, padding...

#### 3.3.2 Components

##### base/

- **BaseDrawer**: nội dung bên phải, mở bằng event
- **BaseIcon**: cấu hình icon chung
- **BaseModal**: cấu hình modal/popup chung
- **ConfirmationModal**: dùng cho xác nhận (extends BaseModal)
- **Loading**: dùng cho loading (uses Overlay)
- **Overlay**: lớp phủ có màu hoặc trong suốt
- **TableSetting**: cấu hình table (extends BaseModal)

##### controls/

- **buttons**: Button, ButtonIcon, ButtonGroup
- **checkboxes**: Checkbox, CheckboxList
- **inputs**: Input, InputTag, Searchbar, FormulaInput (new)
- **pagination**: Pagination
- **selects**: BaseSelect, Select, SelectLabel, SelectMultipleTags, SelectTable
- **tables**: Table, QuickFilterModal

##### drawer/

- **FilterDrawer**: bộ lọc nâng cao

##### form/

- **FormInputRow, FormInputSection, FormSection, ImageForm**

##### icons/

- **ArrowDownIcon, CloseIcon, GripIcon, SettingIcon**

##### others/

- **Tabs, Toast**

#### 3.3.3 Layouts

- **TheHeader**
- **TheSidebar**
  - Tổng quan
  - Thành phần lương (chính)
  - Mẫu bảng lương
  - Dữ liệu tính lương (arrow right, menu: Chấm công, Doanh số, KPI, Sản phẩm, Thu nhập khác, Khấu trừ khác)
  - Tính lương (arrow right, menu: Bảng lương, Tạm ứng, Tổng hợp lương, Phân bổ lương, Ngân sách lương, Bảng thuế, Quyết toán thuế)
  - Chi trả (arrow right, menu: Bảng chi trả, Tổng hợp chi trả)
  - Báo cáo
- **TheContent**
- **TheLayout**
- **ThePageContainer**

#### 3.3.4 Models

- **enums**: ánh xạ backend enums
- **Objects**: tương ứng entities

#### 3.3.5 Services

- **axios-client**: cấu hình axios kết nối backend
- **base-service**: các hàm CRUD entity
- **entity-service**: kế thừa base-service

#### 3.3.6 Utils

- Date formatter
- Currency formatter
- Number formatter
- String utilities
- Toast utilities

#### 3.3.7 Views

##### salary-composition/

**a) SalaryComposition (Danh sách)**

- Header: title "Thành phần lương", nút "Danh mục hệ thống" + "Thêm" (với dropdown)
- Content:
  - Left: thanh tìm kiếm, lọc trạng thái, lọc đơn vị (tree view)
  - Right: nút bộ lọc (FilterDrawer), thiết lập (TableSetting)
  - Table: checkbox, cột. Hover: ngừng/tiếp tục, nhân bản, sửa, xóa. Click row: sửa

- **Actions**:
  - Ngừng/tiếp tục: xác nhận
  - Nhân bản: sang Form
  - Xóa: xác nhận, nếu hệ thống đang dùng → NotificationModal
  - Sửa/click row: sang Form
  - Danh mục hệ thống: sang SystemSalaryComposition
  - Thêm: sang Form
  - Dropdown: chọn từ danh mục hệ thống → AddFromSystemModal
  - Column header: sort/pin column
  - > =1 row: bulk action (ngừng theo dõi, xóa)
  - Filter: tag format `[Cột] [Operator] [Giá trị] [x]`

**b) SystemSalaryComposition (Danh mục hệ thống)**

- Header: "Danh mục thành phần lương của hệ thống"
- Content:
  - Left: thanh tìm kiếm, lọc loại thành phần (type)
  - Right: nút bộ lọc, thiết lập
  - Table: giống SalaryComposition nhưng chỉ có nút "Đưa vào danh sách sử dụng"

- **Actions**:
  - > =1 row: ẩn combobox lọc, hiển thị "Đã chọn X [Bỏ chọn] [Đưa vào danh sách]"

**c) SalaryCompositionForm (Thêm/Sửa)**

- Header: "Thêm thành phần" (chế độ thêm) hoặc tên thành phần (chế độ sửa)

**Form Fields** (chế độ thêm):

- Tên thành phần: required, text
- Mã thành phần: required, text (auto-gen UPPER_SNAKE_CASE từ tên, có thể custom)
- Đơn vị áp dụng: required, select tag (tree view)
- Loại thành phần: required, select
- Tính chất: required, select + conditional section
- Định mức: textarea/FormulaInput + checkbox "Cho phép giá trị vượt quá định mức"
- Kiểu giá trị: select
- Giá trị:
  - Radio: "Tự động cộng tổng" → select (Cùng đơn vị/Dưới quyền/Cơ cấu) + select thành phần lương
  - Radio: "Tính theo công thức" → FormulaInput
- Mô tả: textarea
- Hiển thị trên phiếu lương: radio group (Có/Không/Chỉ nếu khác 0)
- Nguồn tạo: select (default: Tự thêm)

**Conditional Logic**:

- Loại = Sản phẩm/KPI/Chấm công → Tính chất = Khác (Kiểu = Số)
- Tính chất = Thu nhập → radio group 3 nút
- Tính chất = Khấu trừ → checkbox "Giảm trừ khi tính thuế"
- Tính chất = Miễn thuế 1 phần → "Phần chịu thuế" + "Phần miễn thuế" (1 auto-calc)
- Kiểu giá trị = Ngày/Chữ/Phần trăm → chỉ FormulaInput

**Chế độ Sửa**:

- Title: tên thành phần, nút "Sửa" + option (Nhân bản, Xóa)
- Input readonly, radio button vẫn click được. Hover → nút bút sửa riêng dòng
- Nút Sửa top: sửa tất cả trường (form mode)
- Trường được sửa riêng:
  - Tên, Đơn vị, Loại, Tính chất, Định mức, Giá trị, Mô tả: ✓ (sửa riêng)
  - Mã, Kiểu giá trị: ✗ (lock icon)
  - Hiển thị trên phiếu lương: radio có thể change
  - Nguồn tạo, Trạng thái: disable

**Footer**: 3 nút (Hủy bỏ, Lưu và thêm, Lưu)

**Unsaved Changes**: ConfirmationModal "Thoát và không lưu?"

**d) AddFromSystemModal (Thêm từ danh mục)**

- Title: "Thêm từ danh mục hệ thống"
- Content:
  - Thanh tìm kiếm
  - Combobox lọc "Loại thành phần"
  - Table: checkbox, columns. Header: pin/move column
- Footer: "Hủy bỏ" + "Đồng ý" (enable nếu >=1 row)
