/*
 * Database: amis_tienluong_database
 * Description: Hệ thống quản lý Thành phần lương AMIS
 * Convention: GUID (char 36), Snake_case columns
 */

CREATE DATABASE IF NOT EXISTS amis_tienluong_database
CHARACTER SET utf8mb4
COLLATE utf8mb4_0900_as_ci;

USE amis_tienluong_database;

-- 1. Bảng Đơn vị công tác (Catalog)
CREATE TABLE pa_organization (
    organization_id CHAR(36) NOT NULL DEFAULT (UUID()),
    organization_name VARCHAR(255) NOT NULL,
    organization_parent_id CHAR(36) NULL,
    PRIMARY KEY (organization_id),
    CONSTRAINT fk_pa_organization_parent FOREIGN KEY (organization_parent_id) 
        REFERENCES pa_organization(organization_id) ON DELETE NO ACTION
) ENGINE=InnoDB COMMENT='Danh mục đơn vị công tác';

CREATE INDEX ix_organization_parent ON pa_organization(organization_parent_id);

-- 2. Bảng Thành phần lương hệ thống (Reference)
CREATE TABLE pa_salary_composition_system (
    scs_id CHAR(36) NOT NULL DEFAULT (UUID()),
    scs_code VARCHAR(255) NOT NULL,
    scs_name VARCHAR(255) NOT NULL,
    scs_type INT NOT NULL COMMENT '0: Thông tin nhân viên, 1: Chấm công, 2: Doanh số, 3: KPI, 4: Sản phẩm, 5: Lương, 6: Thuế TNCN, 7: Bảo hiểm - Công đoàn, 8: Khác',
    scs_nature INT NOT NULL DEFAULT 0 COMMENT '0: Thu nhập, 1: Khấu trừ, 2: Khác',
    scs_tax_status INT NOT NULL DEFAULT 0 COMMENT '0: Không xác định, 1: Chịu thuế, 2: Miễn thuế toàn phần, 3: Miễn thuế một phần',
    scs_is_tax_deductible TINYINT NOT NULL DEFAULT 0,
    scs_taxable_expression TEXT NULL COMMENT 'Công thức tính phần chịu thuế',
    scs_exempt_expression TEXT NULL COMMENT 'Công thức tính phần miễn thuế',
    scs_limit_expression TEXT NULL COMMENT 'Công thức định mức',
    scs_allow_exceed_limit TINYINT NOT NULL DEFAULT 0,
    scs_value_type INT NOT NULL DEFAULT 0 COMMENT '0: Tiền tệ, 1: Số, 2: Chữ, 3: Ngày, 4: Phần trăm',
    scs_calculation_method INT NOT NULL DEFAULT 0 COMMENT '0: Tự động tính tổng, 1: Tính theo công thức',
    scs_aggregation_scope INT NOT NULL DEFAULT 0 COMMENT '0: Cùng đơn vị, 1: Dưới quyền, 2: Thuộc cơ cấu',
    scs_composition_code VARCHAR(255) NULL COMMENT 'Tham chiếu TPL cha',
    scs_organization_level INT NULL COMMENT 'Cấp tổ chức áp dụng (1-4)',
    scs_formula_expression TEXT NULL COMMENT 'Công thức tính giá trị',
    scs_description VARCHAR(500) DEFAULT '',
    scs_is_displayed_on_payroll INT NOT NULL DEFAULT 0 COMMENT '0: Không, 1: Có, 2: Chỉ nếu khác 0',
    scs_is_deleted TINYINT NOT NULL DEFAULT 0,
    scs_is_in_used TINYINT NOT NULL DEFAULT 0 COMMENT '0: Chưa dùng, 1: Đã đưa vào danh sách sử dụng',
    PRIMARY KEY (scs_id),
    UNIQUE KEY uq_scs_code (scs_code)
) ENGINE=InnoDB COMMENT='Danh mục thành phần lương mẫu của hệ thống';


-- 3. Bảng Thành phần lương của người dùng
CREATE TABLE pa_salary_composition (
    sc_id CHAR(36) NOT NULL DEFAULT (UUID()),
    sc_code VARCHAR(255) NOT NULL,
    sc_name VARCHAR(255) NOT NULL,
    sc_type INT NOT NULL COMMENT '0: Thông tin nhân viên, 1: Chấm công, 2: Doanh số, 3: KPI, 4: Sản phẩm, 5: Lương, 6: Thuế TNCN, 7: Bảo hiểm - Công đoàn, 8: Khác',
    sc_nature INT NOT NULL DEFAULT 0 COMMENT '0: Thu nhập, 1: Khấu trừ, 2: Khác',
    sc_tax_status INT NOT NULL DEFAULT 0 COMMENT '0: Không xác định, 1: Chịu thuế, 2: Miễn thuế toàn phần, 3: Miễn thuế một phần',
    sc_is_tax_deductible TINYINT NOT NULL DEFAULT 0,
    sc_taxable_expression TEXT NULL,
    sc_exempt_expression TEXT NULL,
    sc_limit_expression TEXT NULL,
    sc_allow_exceed_limit TINYINT NOT NULL DEFAULT 0,
    sc_value_type INT NOT NULL DEFAULT 0 COMMENT '0: Tiền tệ, 1: Số, 2: Chữ, 3: Ngày, 4: Phần trăm',
    sc_calculation_method INT NOT NULL DEFAULT 0 COMMENT '0: Tự động tính tổng, 1: Tính theo công thức',
    sc_aggregation_scope INT NOT NULL DEFAULT 0 COMMENT '0: Cùng đơn vị, 1: Dưới quyền, 2: Thuộc cơ cấu',
    sc_composition_code VARCHAR(255) NULL COMMENT 'Tham chiếu TPL cha khi sc_calculation_method = 0',
    sc_organization_level INT NULL COMMENT 'Cấp tổ chức áp dụng (1-4)',
    sc_formula_expression TEXT NULL COMMENT 'Công thức tính giá trị (Dùng khi sc_calculation_method = 1)',
    sc_description VARCHAR(500) DEFAULT '',
    sc_is_displayed_on_payroll INT NOT NULL DEFAULT 0,
    sc_source INT NOT NULL DEFAULT 1 COMMENT '0: Mặc định (Copy từ HT), 1: Tự thêm',
    sc_is_deleted TINYINT NOT NULL DEFAULT 0,
    sc_status INT NOT NULL DEFAULT 0 COMMENT '0: Đang theo dõi, 1: Ngừng theo dõi',
    PRIMARY KEY (sc_id),
    UNIQUE KEY uq_sc_code (sc_code)
) ENGINE=InnoDB COMMENT='Danh sách thành phần lương sử dụng thực tế';

-- 4. Bảng ánh xạ TPL - Đơn vị công tác (Quan hệ Nhiều - Nhiều)
CREATE TABLE salarycomposition_organization (
    sc_id CHAR(36) NOT NULL,
    organization_id CHAR(36) NOT NULL,
    PRIMARY KEY (sc_id, organization_id),
    CONSTRAINT fk_sc_org_composition FOREIGN KEY (sc_id) 
        REFERENCES pa_salary_composition(sc_id) ON DELETE CASCADE,
    CONSTRAINT fk_sc_org_organization FOREIGN KEY (organization_id) 
        REFERENCES pa_organization(organization_id) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Bảng mapping Thành phần lương và Đơn vị áp dụng';

CREATE INDEX ix_org_composition ON salarycomposition_organization(sc_id);
CREATE INDEX ix_org_organization ON salarycomposition_organization(organization_id);

-- 5. Bảng cấu hình Grid (Ghim cột, độ rộng, ẩn hiện)
CREATE TABLE pa_grid_config (
    grid_key VARCHAR(100) NOT NULL COMMENT 'Định danh grid (ví dụ: pa_salary_composition)',
    config_data TEXT NOT NULL COMMENT 'JSON cấu hình các cột',
    PRIMARY KEY (grid_key)
) ENGINE=InnoDB COMMENT='Lưu trữ cấu hình hiển thị bảng của người dùng';
