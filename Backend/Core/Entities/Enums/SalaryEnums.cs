namespace Core.Entities.Enums
{
    /// <summary>
    /// Loại thành phần lương
    /// </summary>
    public enum SalaryCompositionType
    {
        EmployeeInfo = 0,       // Thông tin nhân viên
        Timekeeping = 1,        // Chấm công
        Sales = 2,              // Doanh số
        KPI = 3,                // KPI
        Production = 4,         // Sản phẩm
        Salary = 5,             // Lương
        PersonalIncomeTax = 6,  // Thuế TNCN
        InsuranceAndUnion = 7,  // Bảo hiểm - Công đoàn
        Other = 8               // Khác
    }

    /// <summary>
    /// Tính chất của thành phần lương
    /// </summary>
    public enum SalaryCompositionNature
    {
        Income = 0,             // Thu nhập
        Deduction = 1,          // Khấu trừ
        Other = 2               // Khác
    }

    /// <summary>
    /// Tình trạng chịu thuế TNCN
    /// </summary>
    public enum TaxStatus
    {
        Taxable = 0,            // Chịu thuế
        FullyExempt = 1,        // Miễn thuế toàn phần
        PartiallyExempt = 2     // Miễn thuế một phần
    }

    /// <summary>
    /// Kiểu dữ liệu của giá trị
    /// </summary>
    public enum ValueType
    {
        Currency = 0,           // Tiền tệ
        Number = 1,             // Số
        Text = 2,               // Chữ
        Date = 3,               // Ngày
        Percentage = 4          // Phần trăm
    }

    /// <summary>
    /// Phương thức tính toán
    /// </summary>
    public enum CalculationMethod
    {
        AutoSum = 0,            // Tự động tính tổng
        Formula = 1             // Tính theo công thức
    }

    /// <summary>
    /// Phạm vi tổng hợp dữ liệu
    /// </summary>
    public enum AggregationScope
    {
        SameOrganization = 0,   // Cùng đơn vị
        Subordinates = 1,       // Dưới quyền
        Structure = 2           // Thuộc cơ cấu
    }

    /// <summary>
    /// Cấu hình hiển thị trên phiếu lương
    /// </summary>
    public enum DisplayOnPayroll
    {
        No = 0,
        Yes = 1,
        OnlyIfNonZero = 2       // Chỉ hiện nếu khác 0
    }

    /// <summary>
    /// Trạng thái theo dõi của người dùng
    /// </summary>
    public enum SalaryStatus
    {
        Active = 0,             // Đang theo dõi
        Inactive = 1            // Ngừng theo dõi
    }

    /// <summary>
    /// Nguồn gốc của thành phần lương người dùng
    /// </summary>
    public enum SalaryCompositionSource
    {
        SystemDefault = 0,      // Copy từ hệ thống
        UserAdded = 1           // Tự thêm bởi người dùng
    }
}