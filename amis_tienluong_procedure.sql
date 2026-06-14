/*
 * Database: amis_tienluong_database
 * Convention: 
 * - Parameters: p_column_name
 * - Variables: v_variable_name
 */

CREATE DATABASE IF NOT EXISTS amis_tienluong_database
CHARACTER SET utf8mb4
COLLATE utf8mb4_0900_as_ci;

USE amis_tienluong_database;

DELIMITER $$

/* =================================================================================
 * 1. GROUP: DANH MỤC (GET ONLY)
 * ================================================================================= */

-- Lấy danh sách đơn vị công tác
CREATE PROCEDURE proc_get_pa_organization()
BEGIN
    SELECT * FROM pa_organization;
END$$

-- Lấy danh sách thành phần lương mẫu từ hệ thống
CREATE PROCEDURE proc_get_pa_salary_composition_system_paging(
    IN p_search_term VARCHAR(255),
    IN p_where_clause TEXT,
    IN p_page_index INT,
    IN p_page_size INT
)
BEGIN
    DECLARE v_offset INT;
    SET v_offset = (p_page_index - 1) * p_page_size;

    -- 1. Khởi tạo mệnh đề WHERE
    -- Standard: scs_is_deleted = 0 và scs_is_in_used = 0 (chỉ lấy mẫu chưa dùng)
    SET @v_where = ' WHERE scs_is_deleted = 0 AND scs_is_in_used = 0 ';

    IF p_search_term IS NOT NULL AND p_search_term <> '' THEN
        SET @v_search = CONCAT('%', p_search_term, '%');
        SET @v_where = CONCAT(@v_where, ' AND (scs_code LIKE ', QUOTE(@v_search), ' OR scs_name LIKE ', QUOTE(@v_search), ') ');
    END IF;

    -- Xử lý p_where_clause: Sử dụng IFNULL để tránh làm hỏng chuỗi CONCAT nếu tham số null
    SET @v_where = CONCAT(@v_where, IFNULL(p_where_clause, ''));

    -- ResultSet 1: Trả về tổng số bản ghi (Standard: Luôn trả về Count trước)
    SET @v_count_sql = CONCAT('SELECT COUNT(*) FROM pa_salary_composition_system ', @v_where);
    PREPARE stmt_count FROM @v_count_sql;
    EXECUTE stmt_count;
    DEALLOCATE PREPARE stmt_count;

    -- ResultSet 2: Trả về dữ liệu trang hiện tại (Items)
    SET @v_data_sql = CONCAT('
        SELECT * FROM pa_salary_composition_system ', 
        @v_where, 
        ' ORDER BY scs_code ASC LIMIT ', p_page_size, ' OFFSET ', v_offset);
    PREPARE stmt_data FROM @v_data_sql;
    EXECUTE stmt_data;
    DEALLOCATE PREPARE stmt_data;
END$$

/* =================================================================================
 * 2. GROUP: CRUD PA_SALARY_COMPOSITION
 * ================================================================================= */

-- Thêm mới Thành phần lương
CREATE PROCEDURE proc_insert_pa_salary_composition(
    IN p_sc_id CHAR(36),
    IN p_sc_code VARCHAR(255),
    IN p_sc_name VARCHAR(255),
    IN p_sc_type INT,
    IN p_sc_nature INT,
    IN p_sc_tax_status INT,
    IN p_sc_is_tax_deductible TINYINT,
    IN p_sc_taxable_expression TEXT,
    IN p_sc_exempt_expression TEXT,
    IN p_sc_limit_expression TEXT,
    IN p_sc_allow_exceed_limit TINYINT,
    IN p_sc_value_type INT,
    IN p_sc_calculation_method INT,
    IN p_sc_aggregation_scope INT,
    IN p_sc_composition_code VARCHAR(255),
    IN p_sc_organization_level INT,
    IN p_sc_formula_expression TEXT,
    IN p_sc_description VARCHAR(500),
    IN p_sc_is_displayed_on_payroll INT,
    IN p_sc_source INT
)
BEGIN
    INSERT INTO pa_salary_composition (
        sc_id, sc_code, sc_name, sc_type, sc_nature, sc_tax_status,
        sc_is_tax_deductible, sc_taxable_expression, sc_exempt_expression,
        sc_limit_expression, sc_allow_exceed_limit, sc_value_type,
        sc_calculation_method, sc_aggregation_scope, sc_composition_code,
        sc_organization_level, sc_formula_expression, sc_description,
        sc_is_displayed_on_payroll, sc_source, sc_is_deleted, sc_status
    ) VALUES (
        p_sc_id, p_sc_code, p_sc_name, p_sc_type, p_sc_nature, p_sc_tax_status,
        p_sc_is_tax_deductible, p_sc_taxable_expression, p_sc_exempt_expression,
        p_sc_limit_expression, p_sc_allow_exceed_limit, p_sc_value_type,
        p_sc_calculation_method, p_sc_aggregation_scope, p_sc_composition_code,
        p_sc_organization_level, p_sc_formula_expression, p_sc_description,
        p_sc_is_displayed_on_payroll, p_sc_source, 0, 0
    );
END$$

-- Cập nhật Thành phần lương
CREATE PROCEDURE proc_update_pa_salary_composition(
    IN p_sc_id CHAR(36),
    IN p_sc_name VARCHAR(255),
    IN p_sc_type INT,
    IN p_sc_nature INT,
    IN p_sc_tax_status INT,
    IN p_sc_is_tax_deductible TINYINT,
    IN p_sc_taxable_expression TEXT,
    IN p_sc_exempt_expression TEXT,
    IN p_sc_limit_expression TEXT,
    IN p_sc_allow_exceed_limit TINYINT,
    IN p_sc_value_type INT,
    IN p_sc_calculation_method INT,
    IN p_sc_aggregation_scope INT,
    IN p_sc_composition_code VARCHAR(255),
    IN p_sc_organization_level INT,
    IN p_sc_formula_expression TEXT,
    IN p_sc_description VARCHAR(500),
    IN p_sc_is_displayed_on_payroll INT,
    IN p_sc_status INT
)
BEGIN
    UPDATE pa_salary_composition SET
        sc_name = p_sc_name,
        sc_type = p_sc_type,
        sc_nature = p_sc_nature,
        sc_tax_status = p_sc_tax_status,
        sc_is_tax_deductible = p_sc_is_tax_deductible,
        sc_taxable_expression = p_sc_taxable_expression,
        sc_exempt_expression = p_sc_exempt_expression,
        sc_limit_expression = p_sc_limit_expression,
        sc_allow_exceed_limit = p_sc_allow_exceed_limit,
        sc_value_type = p_sc_value_type,
        sc_calculation_method = p_sc_calculation_method,
        sc_aggregation_scope = p_sc_aggregation_scope,
        sc_composition_code = p_sc_composition_code,
        sc_organization_level = p_sc_organization_level,
        sc_formula_expression = p_sc_formula_expression,
        sc_description = p_sc_description,
        sc_is_displayed_on_payroll = p_sc_is_displayed_on_payroll,
        sc_status = p_sc_status
    WHERE sc_id = p_sc_id;
END$$

-- Xóa Thành phần lương (Soft delete)
CREATE PROCEDURE proc_delete_pa_salary_composition(
    IN p_sc_id CHAR(36)
)
BEGIN
    UPDATE pa_salary_composition 
    SET sc_is_deleted = 1 
    WHERE sc_id = p_sc_id;
END$$

-- Lấy chi tiết một Thành phần lương
CREATE PROCEDURE proc_get_pa_salary_composition_by_id(
    IN p_sc_id CHAR(36)
)
BEGIN
    SELECT sc.*
    FROM pa_salary_composition sc
    WHERE sc.sc_id = p_sc_id AND sc.sc_is_deleted = 0;

    -- Lấy danh sách ID và Tên đơn vị áp dụng
    SELECT 
        sco.organization_id AS OrganizationId, 
        o.organization_name AS OrganizationName 
    FROM salarycomposition_organization sco
    JOIN pa_organization o ON sco.organization_id = o.organization_id
    WHERE sco.sc_id = p_sc_id;
END$$

/* =================================================================================
 * 3. GROUP: NGHIỆP VỤ ĐẶC THÙ
 * ================================================================================= */

-- Phân trang danh sách TPL (Việc lọc chi tiết sẽ do C# xây dựng query)
CREATE PROCEDURE proc_get_pa_salary_composition_paging(
    IN p_search_term VARCHAR(255),
    IN p_where_clause TEXT, -- C# có thể truyền thêm điều kiện nếu cần hoặc xử lý hoàn toàn ở code
    IN p_page_index INT,
    IN p_page_size INT
)
BEGIN
    DECLARE v_offset INT;
    SET v_offset = (p_page_index - 1) * p_page_size;

    -- Tạo bảng tạm để lưu ID của trang hiện tại
    CREATE TEMPORARY TABLE IF NOT EXISTS temp_sc_ids (sc_id CHAR(36));
    DELETE FROM temp_sc_ids;

    -- 1. Khởi tạo mệnh đề WHERE
    SET @v_where = ' WHERE sc.sc_is_deleted = 0 ';

    IF p_search_term IS NOT NULL AND p_search_term <> '' THEN
        SET @v_search = CONCAT('%', p_search_term, '%');
        SET @v_where = CONCAT(@v_where, ' AND (sc.sc_code LIKE ', QUOTE(@v_search), ' OR sc.sc_name LIKE ', QUOTE(@v_search), ') ');
    END IF;

    IF p_where_clause IS NOT NULL AND p_where_clause <> '' THEN
        SET @v_where = CONCAT(@v_where, ' ', p_where_clause);
    END IF;

    -- 2. Đổ ID của trang vào bảng tạm
    SET @v_temp_sql = CONCAT('
        INSERT INTO temp_sc_ids 
        SELECT sc.sc_id 
        FROM pa_salary_composition sc 
        LEFT JOIN salarycomposition_organization sco ON sc.sc_id = sco.sc_id ', 
        @v_where, 
        ' GROUP BY sc.sc_id 
        ORDER BY sc.sc_code ASC LIMIT ', p_page_size, ' OFFSET ', v_offset);

    PREPARE stmt_temp FROM @v_temp_sql;
    EXECUTE stmt_temp;
    DEALLOCATE PREPARE stmt_temp;

    -- ResultSet 1: Lấy tổng số bản ghi (Total Count)
    SET @v_count_sql = CONCAT('
        SELECT COUNT(DISTINCT sc.sc_id) 
        FROM pa_salary_composition sc 
        LEFT JOIN salarycomposition_organization sco ON sc.sc_id = sco.sc_id ', 
        @v_where);
    
    PREPARE stmt_count FROM @v_count_sql;
    EXECUTE stmt_count;
    DEALLOCATE PREPARE stmt_count;

    -- ResultSet 2: Lấy dữ liệu phân trang
    SET @v_data_sql = CONCAT('
        SELECT sc.*
        FROM pa_salary_composition sc
        INNER JOIN temp_sc_ids t ON sc.sc_id = t.sc_id
        ORDER BY sc.sc_code ASC');

    PREPARE stmt_data FROM @v_data_sql;
    EXECUTE stmt_data;
    DEALLOCATE PREPARE stmt_data;

    -- ResultSet 3: Lấy mapping kèm tên đơn vị
    SELECT 
        sco.sc_id AS ScId, 
        sco.organization_id AS OrganizationId, 
        o.organization_name AS OrganizationName 
    FROM salarycomposition_organization sco
    JOIN pa_organization o ON sco.organization_id = o.organization_id
    WHERE sco.sc_id IN (SELECT sc_id FROM temp_sc_ids);
END$$
-- Sao chép hàng loạt TPL từ hệ thống sang danh sách sử dụng
CREATE PROCEDURE proc_bulk_clone_system_salary_composition(
    IN p_scs_ids TEXT
)
BEGIN
    DECLARE v_root_org_id CHAR(36);

    -- 1. Lấy ID của đơn vị cấp 0 (node gốc - đơn vị không có đơn vị cha)
    SELECT organization_id INTO v_root_org_id
    FROM pa_organization
    WHERE organization_parent_id IS NULL OR organization_parent_id = ''
    LIMIT 1;

    -- Tạo bảng tạm để lưu trữ cặp ID (Hệ thống - Người dùng mới) nhằm mục đích map đơn vị và update trạng thái
    CREATE TEMPORARY TABLE IF NOT EXISTS temp_clone_map (
        scs_id CHAR(36),
        new_sc_id CHAR(36)
    );
    DELETE FROM temp_clone_map;

    -- Xác định danh sách các bản ghi hợp lệ để clone và sinh UUID mới trước
    INSERT INTO temp_clone_map (scs_id, new_sc_id)
    SELECT scs_id, UUID()
    FROM pa_salary_composition_system
    WHERE FIND_IN_SET(scs_id, p_scs_ids) 
      AND scs_is_deleted = 0 
      AND scs_is_in_used = 0
      AND scs_code NOT IN (SELECT sc_code FROM pa_salary_composition WHERE sc_is_deleted = 0);

    -- 2. Insert vào bảng pa_salary_composition sử dụng ID đã sinh ở bước trên
    INSERT INTO pa_salary_composition (
        sc_id, sc_code, sc_name, sc_type, sc_nature, sc_tax_status,
        sc_is_tax_deductible, sc_taxable_expression, sc_exempt_expression,
        sc_limit_expression, sc_allow_exceed_limit, sc_value_type, sc_calculation_method,
        sc_aggregation_scope, sc_composition_code, sc_organization_level,
        sc_formula_expression, sc_description, sc_is_displayed_on_payroll, 
        sc_source, sc_is_deleted, sc_status
    )
    SELECT 
        m.new_sc_id, s.scs_code, s.scs_name, s.scs_type, s.scs_nature, s.scs_tax_status,
        s.scs_is_tax_deductible, s.scs_taxable_expression, s.scs_exempt_expression,
        s.scs_limit_expression, s.scs_allow_exceed_limit, s.scs_value_type, s.scs_calculation_method,
        s.scs_aggregation_scope, s.scs_composition_code, s.scs_organization_level,
        s.scs_formula_expression, s.scs_description, s.scs_is_displayed_on_payroll, 
        0, 0, 0
    FROM pa_salary_composition_system s
    JOIN temp_clone_map m ON s.scs_id = m.scs_id;

    -- 3. Tự động gán đơn vị mặc định (Organization cấp 0) cho các TPL vừa được tạo
    IF v_root_org_id IS NOT NULL THEN
        INSERT INTO salarycomposition_organization (sc_id, organization_id)
        SELECT new_sc_id, v_root_org_id
        FROM temp_clone_map;
    END IF;

    -- 4. Đánh dấu các TPL hệ thống đã được sử dụng
    UPDATE pa_salary_composition_system scs
    JOIN temp_clone_map m ON scs.scs_id = m.scs_id
    SET scs.scs_is_in_used = 1;
      
    -- Trả về số lượng bản ghi đã clone thành công
    SELECT COUNT(*) AS AffectedRows FROM temp_clone_map;

    DROP TEMPORARY TABLE temp_clone_map;
END$$

-- Cập nhật danh sách đơn vị áp dụng (Upsert Mapping)
CREATE PROCEDURE proc_upsert_salarycomposition_organization(
    IN p_sc_id CHAR(36),
    IN p_organization_ids TEXT -- Nhận danh sách ID cách nhau bởi dấu phẩy
)
BEGIN
    -- 1. Xóa bỏ các ánh xạ cũ của TPL này (để phục vụ cả tính năng Update)
    DELETE FROM salarycomposition_organization WHERE sc_id = p_sc_id;

    -- 2. Tách chuỗi CSV và insert vào bảng mapping
    IF p_organization_ids IS NOT NULL AND p_organization_ids <> '' THEN
        INSERT INTO salarycomposition_organization (sc_id, organization_id)
        SELECT p_sc_id, org_id
        FROM JSON_TABLE(
            CONCAT('["', REPLACE(p_organization_ids, ',', '","'), '"]'),
            "$[*]" COLUMNS (org_id CHAR(36) PATH "$")
        ) AS jt;
    END IF;
END$$

/* =================================================================================
 * 4. GROUP: BULK ACTIONS & GRID CONFIG
 * ================================================================================= */

-- Xóa hàng loạt (Soft delete)
CREATE PROCEDURE proc_bulk_delete_pa_salary_composition(
    IN p_ids TEXT -- Danh sách ID cách nhau bởi dấu phẩy
)
BEGIN
    UPDATE pa_salary_composition 
    SET sc_is_deleted = 1 
    WHERE FIND_IN_SET(sc_id, p_ids) AND sc_source = 1;
END$$

-- Cập nhật trạng thái hàng loạt
CREATE PROCEDURE proc_bulk_update_status_pa_salary_composition(
    IN p_ids TEXT,
    IN p_status INT
)
BEGIN
    UPDATE pa_salary_composition 
    SET sc_status = p_status
    WHERE FIND_IN_SET(sc_id, p_ids);
END$$

-- Lấy cấu hình Grid
CREATE PROCEDURE proc_get_pa_grid_config(
    IN p_grid_key VARCHAR(100)
)
BEGIN
    SELECT config_data 
    FROM pa_grid_config 
    WHERE grid_key = p_grid_key;
END$$

-- Lưu cấu hình Grid (Upsert)
CREATE PROCEDURE proc_upsert_pa_grid_config(
    IN p_grid_key VARCHAR(100),
    IN p_config_data TEXT
)
BEGIN
    INSERT INTO pa_grid_config (grid_key, config_data)
    VALUES (p_grid_key, p_config_data)
    ON DUPLICATE KEY UPDATE 
    config_data = p_config_data;
END$$

-- Lấy danh sách TPL rút gọn phục vụ lookup công thức (có tìm kiếm)
CREATE PROCEDURE proc_get_pa_salary_composition_lookup(
    IN p_search_term VARCHAR(255)
)
BEGIN
    SELECT 
        sc_id AS ScId, 
        sc_code AS ScCode, 
        sc_name AS ScName 
    FROM pa_salary_composition 
    WHERE sc_is_deleted = 0 
      AND (p_search_term IS NULL OR p_search_term = '' OR sc_code LIKE CONCAT('%', p_search_term, '%') OR sc_name LIKE CONCAT('%', p_search_term, '%'))
    ORDER BY sc_code ASC;
END$$

-- Kiểm tra trùng mã Thành phần lương (loại trừ ID hiện tại nếu là update)
CREATE PROCEDURE proc_check_salary_composition_code_duplicate(
    IN p_sc_code VARCHAR(255),
    IN p_sc_id CHAR(36)
)
BEGIN
    SELECT EXISTS(
        SELECT 1 
        FROM pa_salary_composition 
        WHERE sc_code = p_sc_code 
          AND (p_sc_id IS NULL OR sc_id <> p_sc_id)
          AND sc_is_deleted = 0
    ) AS is_duplicate;
END$$

DELIMITER ;
