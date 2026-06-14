USE amis_tienluong_database;

-- 2. Dữ liệu mẫu cho pa_organization (Đơn vị công tác)
INSERT INTO pa_organization (organization_id, organization_name, organization_parent_id) VALUES
('660e8400-e29b-41d4-a716-446655440000', 'Công ty Cổ phần MISA', NULL),
('660e8400-e29b-41d4-a716-446655440001', 'Khối Sản xuất', '660e8400-e29b-41d4-a716-446655440000'),
('660e8400-e29b-41d4-a716-446655440002', 'Khối Kinh doanh', '660e8400-e29b-41d4-a716-446655440000'),
('660e8400-e29b-41d4-a716-446655440003', 'Trung tâm Phát triển Phần mềm', '660e8400-e29b-41d4-a716-446655440001'),
('660e8400-e29b-41d4-a716-446655440004', 'Văn phòng Tư vấn Giải pháp', '660e8400-e29b-41d4-a716-446655440002');

-- 3. Dữ liệu mẫu cho pa_salary_composition_system (TPL Hệ thống)
INSERT INTO pa_salary_composition_system (scs_id, scs_code, scs_name, scs_type, scs_nature, scs_tax_status, scs_is_tax_deductible, scs_taxable_expression, scs_exempt_expression, scs_limit_expression, scs_value_type, scs_calculation_method, scs_aggregation_scope, scs_composition_code, scs_organization_level, scs_formula_expression, scs_description, scs_is_displayed_on_payroll, scs_is_deleted, scs_is_in_used) VALUES
('770e8400-e29b-41d4-a716-446655440000', 'L_CO_BAN', 'Lương cơ bản', 0, 0, 1, 0, NULL, NULL, NULL, 0, 1, 0, 'L_CO_BAN', NULL, '[L_HOP_DONG]', 'Lương cơ bản tính theo lương hợp đồng', 1, 0, 1),
('770e8400-e29b-41d4-a716-446655440001', 'PC_AN_TRUA', 'Phụ cấp ăn trưa', 1, 0, 3, 0, '{Total} - 730000', '730000', '730000', 0, 1, 0, 'PC_AN_TRUA', NULL, '[NGAY_CONG] * 35000', 'Phụ cấp ăn trưa có định mức miễn thuế theo quy định', 1, 0, 1),
('770e8400-e29b-41d4-a716-446655440002', 'BHXH_NV', 'Bảo hiểm xã hội (NV đóng)', 2, 1, 0, 1, NULL, NULL, NULL, 0, 1, 0, 'BHXH_NV', NULL, '[LUONG_CB] * OTH%', 'Khấu trừ bảo hiểm xã hội tỷ lệ 8%', 1, 0, 1);

-- 4. Dữ liệu mẫu cho pa_salary_composition (TPL Người dùng)
-- C1: Lương cơ bản (Copy từ hệ thống)
-- C2: Phụ cấp ăn trưa (Copy từ hệ thống, có định mức)
-- C3: BHXH (Copy từ hệ thống, khấu trừ)
-- C4: Thưởng dự án (Tự thêm)
-- C5: Tổng thu nhập (Tự thêm, Tính theo công thức = C1 + C2 + C4)
INSERT INTO pa_salary_composition (sc_id, sc_code, sc_name, sc_type, sc_nature, sc_tax_status, sc_is_tax_deductible, sc_taxable_expression, sc_exempt_expression, sc_limit_expression, sc_allow_exceed_limit, sc_value_type, sc_calculation_method, sc_aggregation_scope, sc_composition_code, sc_organization_level, sc_formula_expression, sc_description, sc_is_displayed_on_payroll, sc_source, sc_is_deleted, sc_status) VALUES
('880e8400-e29b-41d4-a716-446655440000', 'LUONG_CB', 'Lương cơ bản', 0, 0, 1, 0, NULL, NULL, NULL, 0, 0, 1, 0, NULL, NULL, '[L_HOP_DONG]', 'Lương cơ bản theo hợp đồng nhân viên', 1, 0, 0, 0),
('880e8400-e29b-41d4-a716-446655440001', 'PC_AN_TRUA', 'Phụ cấp ăn trưa', 1, 0, 3, 0, '{Total} - 730000', '730000', '730000', 0, 0, 1, 0, NULL, NULL, '[NGAY_CONG] * 35000', 'Phụ cấp ăn trưa (miễn thuế tối đa 730k)', 1, 0, 0, 0),
('880e8400-e29b-41d4-a716-446655440002', 'BHXH_NV', 'Bảo hiểm xã hội', 2, 1, 0, 1, NULL, NULL, NULL, 0, 0, 1, 0, 'BHXH_NV', NULL, '[LUONG_CB] * 0.08', 'Khấu trừ bảo hiểm xã hội tỷ lệ 8%', 1, 0, 0, 0),
('880e8400-e29b-41d4-a716-446655440003', 'THUONG_DA', 'Thưởng dự án', 8, 0, 1, 0, NULL, NULL, NULL, 0, 0, 1, 0, NULL, NULL, '0', 'Thưởng dự án cho bộ phận kỹ thuật', 1, 1, 0, 0),
('880e8400-e29b-41d4-a716-446655440004', 'TONG_THU_NHAP', 'Tổng thu nhập', 5, 0, 0, 0, NULL, NULL, NULL, 0, 0, 1, 0, NULL, NULL, '[LUONG_CB] + [PC_AN_TRUA] + [THUONG_DA]', 'Tổng các khoản thu nhập thực nhận', 1, 1, 0, 0);

-- Phụ cấp ăn trưa áp dụng cho Khối Sản xuất
INSERT INTO salarycomposition_organization (sc_id, organization_id) VALUES
('880e8400-e29b-41d4-a716-446655440001', '660e8400-e29b-41d4-a716-446655440001'),
-- BHXH áp dụng cho toàn công ty
('880e8400-e29b-41d4-a716-446655440002', '660e8400-e29b-41d4-a716-446655440000'),
-- Thưởng dự án chỉ áp dụng cho Phòng Dev
('880e8400-e29b-41d4-a716-446655440003', '660e8400-e29b-41d4-a716-446655440003'),
-- Tổng thu nhập áp dụng cho toàn công ty
('880e8400-e29b-41d4-a716-446655440004', '660e8400-e29b-41d4-a716-446655440000');

-- 6. Dữ liệu mẫu cho pa_grid_config
INSERT INTO pa_grid_config (grid_key, config_data) VALUES
('pa_salary_composition', '{"columns":[{"key":"sc_code","visible":true,"width":200},{"key":"sc_name","visible":true,"width":200}]}'),
('pa_salary_composition_system', '{"columns":[{"key":"scs_code","visible":true,"width":180},{"key":"scs_name","visible":true,"width":220}]}');