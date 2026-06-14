import { ref, computed, watch, onMounted, nextTick, reactive } from 'vue'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import CopyIcon from '@/components/icons/CopyIcon.vue'
import TrashIcon from '@/components/icons/TrashIcon.vue'
import salaryCompositionService from '@/services/salary-composition-service'
import {
  SalaryCompositionNature,
  SalaryCompositionType,
  TaxStatus,
  ValueType,
  SalaryCompositionSource,
  DisplayOnPayroll,
  AggregationScope,
} from '@/models/SalaryEnums'
import organizationService from '@/services/organization-service'
import { useToast } from '@/utils/use-toast'

export default function useSalaryCompositionForm(props, { emit }) {
  const { showToast } = useToast()

  // Computed xác định form đang ở chế độ Sửa (có ID) hay Thêm mới.
  const isEditMode = computed(() => !!props.compositionId)

  // Flag để kiểm soát việc hiển thị lỗi validation (chỉ sau khi nhấn Lưu)
  const formSubmitted = ref(false)

  // Schema định nghĩa các quy tắc validation cho từng trường dữ liệu bằng Yup
  const validationSchema = yup.object({
    sc_name: yup.string().required('Tên thành phần không được để trống'),
    sc_code: yup.string().required('Mã thành phần không được để trống'),
    organization_ids: yup.array().min(1, 'Vui lòng chọn ít nhất một đơn vị áp dụng'),
    sc_type: yup
      .number()
      .typeError('Vui lòng chọn loại thành phần')
      .required('Vui lòng chọn loại thành phần'),
    sc_nature: yup.number().required(),
    sc_calculation_method: yup.number().required(),
    // Validate phụ thuộc: Nếu là Tự động cộng tổng (0) thì sc_composition_code bắt buộc
    sc_composition_code: yup
      .string()
      .nullable()
      .when('sc_calculation_method', {
        is: 0,
        then: (schema) => schema.required('Vui lòng chọn thành phần lương để cộng giá trị'),
        otherwise: (schema) => schema.notRequired(),
      }),
    // Validate phụ thuộc: Nếu Miễn thuế 1 phần thì phải nhập ít nhất 1 trong 2 công thức thuế
    sc_taxable_expression: yup
      .string()
      .nullable()
      .test(
        'tax-required',
        'Vui lòng nhập công thức tính phần chịu thuế hoặc miễn thuế',
        function (value) {
          const { sc_nature, sc_tax_status, sc_exempt_expression } = this.parent
          if (sc_nature === 0 && sc_tax_status === TaxStatus.PartiallyExempt) {
            return !!value || !!sc_exempt_expression
          }
          return true
        },
      ),
    sc_exempt_expression: yup
      .string()
      .nullable()
      .test(
        'tax-required-exempt',
        'Vui lòng nhập công thức tính phần chịu thuế hoặc miễn thuế',
        function (value) {
          const { sc_nature, sc_tax_status, sc_taxable_expression } = this.parent
          if (sc_nature === 0 && sc_tax_status === TaxStatus.PartiallyExempt) {
            return !!value || !!sc_taxable_expression
          }
          return true
        },
      ),
  })

  /**
   * Khởi tạo VeeValidate useForm
   */
  const {
    defineField,
    errors,
    validate: validateForm,
    resetForm: resetVeeForm,
    setErrors,
    setFieldValue,
    meta,
  } = useForm({
    validationSchema, // Gán schema validation
    // Các cờ cấu hình để không validate ngay khi gõ/blur mà đợi nhấn Lưu
    validateOnBlur: false,
    validateOnChange: false,
    validateOnMount: false,
    validateOnModelUpdate: false,
    initialValues: {
      sc_name: '',
      sc_code: '',
      organization_ids: [],
      sc_type: null,
      sc_nature: 0,
      sc_is_tax_deductible: 0,
      sc_tax_status: 0,
      sc_taxable_expression: '',
      sc_exempt_expression: '',
      sc_limit_expression: '',
      sc_allow_exceed_limit: 0,
      sc_value_type: 0,
      sc_calculation_method: 1,
      sc_aggregation_scope: 0,
      sc_organization_level: null,
      sc_composition_code: null,
      sc_formula_expression: '',
      sc_description: '',
      sc_is_displayed_on_payroll: 1,
      sc_source: 1,
      sc_status: 0,
    },
  })

  // Liên kết các trường trong VeeValidate với biến cục bộ để dùng v-model trong template
  const [sc_name] = defineField('sc_name')
  const [sc_code] = defineField('sc_code')
  const [organization_ids] = defineField('organization_ids')
  const [sc_type] = defineField('sc_type')
  const [sc_nature] = defineField('sc_nature')
  const [sc_is_tax_deductible] = defineField('sc_is_tax_deductible')
  const [sc_tax_status] = defineField('sc_tax_status')
  const [sc_limit_expression] = defineField('sc_limit_expression')
  const [sc_allow_exceed_limit] = defineField('sc_allow_exceed_limit')
  const [sc_value_type] = defineField('sc_value_type')
  const [sc_calculation_method] = defineField('sc_calculation_method')
  const [sc_aggregation_scope] = defineField('sc_aggregation_scope')
  const [sc_organization_level] = defineField('sc_organization_level')
  const [sc_composition_code] = defineField('sc_composition_code')
  const [sc_formula_expression] = defineField('sc_formula_expression')
  const [sc_description] = defineField('sc_description')
  const [sc_is_displayed_on_payroll] = defineField('sc_is_displayed_on_payroll')
  const [sc_source] = defineField('sc_source')
  const [sc_status] = defineField('sc_status')

  // Đối tượng phản ứng chứa toàn bộ dữ liệu form để template truy cập đồng nhất qua form.xxx
  const form = reactive({
    sc_name,
    sc_code,
    organization_ids,
    sc_type,
    sc_nature,
    sc_is_tax_deductible,
    sc_tax_status,
    sc_limit_expression,
    sc_allow_exceed_limit,
    sc_value_type,
    sc_calculation_method,
    sc_aggregation_scope,
    sc_organization_level,
    sc_composition_code,
    sc_formula_expression,
    sc_description,
    sc_is_displayed_on_payroll,
    sc_source,
    sc_status,
  })

  // Trạng thái chỉ đọc của form.
  // Mặc định là true (khóa) nếu vào từ danh sách (có ID), false (mở) nếu là thêm mới.
  const isReadOnly = ref(!!props.compositionId)

  // Trạng thái modal cảnh báo khi thoát form có dữ liệu chưa lưu
  const isUnsavedChangesModalVisible = ref(false)

  // Lưu lại hành động người dùng muốn thực hiện sau khi xác nhận thoát ('close' hoặc 'revert')
  const pendingExitAction = ref(null)

  // Các computed trả về thông báo lỗi nếu form đã được submit
  const nameError = computed(() => (formSubmitted.value ? errors.value.sc_name : ''))
  const orgIdsError = computed(() => (formSubmitted.value ? errors.value.organization_ids : ''))
  const typeError = computed(() => (formSubmitted.value ? errors.value.sc_type : ''))
  const natureError = computed(() => (formSubmitted.value ? errors.value.sc_nature : ''))
  const compCodeError = computed(() =>
    formSubmitted.value ? errors.value.sc_composition_code : '',
  )
  const formulaExprError = computed(() =>
    formSubmitted.value ? errors.value.sc_formula_expression : '',
  )
  const taxableError = computed(() =>
    formSubmitted.value ? errors.value.sc_taxable_expression : '',
  )
  const exemptError = computed(() => (formSubmitted.value ? errors.value.sc_exempt_expression : ''))

  // Lưu trữ dữ liệu gốc khi load từ API để phục vụ chức năng "Hủy bỏ" (revert dữ liệu)
  const originalForm = ref(null)

  // Tiêu đề form thay đổi theo trạng thái (Tên thành phần lương hoặc "Thêm mới")
  const formTitle = computed(() => {
    return isEditMode.value ? originalForm.value?.sc_name || 'Đang tải...' : 'Thêm thành phần lương'
  })
  const isDeleteModalVisible = ref(false)
  /**
   * Menu con cho chế độ xem
   */
  const moreActions = computed(() => [
    {
      label: 'Nhân bản',
      icon: CopyIcon,
      command: handleDuplicate,
    },
    {
      label: 'Xóa',
      icon: TrashIcon,
      command: () => {
        if (form.sc_source === SalaryCompositionSource.SystemDefault) {
          emit(
            'error-notification',
            'Đây là thành phần lương mặc định của hệ thống nên không thể xóa. Vui lòng kiểm tra lại.',
          )
        } else {
          isDeleteModalVisible.value = true
        }
      },
    },
  ])
  const hasUserEditedCode = ref(false)

  // Biến chứa lỗi trùng mã từ API (quản lý riêng vì check async liên tục)
  const duplicateCodeError = ref(null)

  // Cờ tránh vòng lặp watcher khi hệ thống đang cập nhật mã tự động
  const isUpdatingCodeInternally = ref(false)

  // Cờ trạng thái đang load dữ liệu ban đầu
  const isInitialLoading = ref(false)

  // Ref cho input đầu tiên để auto-focus
  const firstInputRef = ref(null)

  const DEFAULT_TAX_PLACEHOLDER = 'Chỉ cần nhập giá trị cho 1 trong 2 phần chịu thuế và miễn thuế'

  /**
   * Computed property để xác định trạng thái disabled của trường "Phần chịu thuế".
   * Disabled nếu form đang ở chế độ chỉ đọc HOẶC nếu trường "Phần miễn thuế" đã có giá trị.
   */
  const isTaxableDisabled = computed(() => {
    return (
      isReadOnly.value ||
      (!!form.sc_exempt_expression && form.sc_exempt_expression.trim().length > 0)
    )
  })

  /**
   * Computed property để xác định trạng thái disabled của trường "Phần miễn thuế".
   * Disabled nếu form đang ở chế độ chỉ đọc HOẶC nếu trường "Phần chịu thuế" đã có giá trị.
   */
  const isExemptDisabled = computed(() => {
    return (
      isReadOnly.value ||
      (!!form.sc_taxable_expression && form.sc_taxable_expression.trim().length > 0)
    )
  })

  /**
   * Placeholder động cho Phần chịu thuế
   */
  const taxablePlaceholder = computed(() => {
    if (isTaxableDisabled.value && !isReadOnly.value) {
      return 'Tổng giá trị - Phần miễn thuế'
    }
    return DEFAULT_TAX_PLACEHOLDER
  })

  /**
   * Placeholder động cho Phần miễn thuế
   */
  const exemptPlaceholder = computed(() => {
    if (isExemptDisabled.value && !isReadOnly.value) {
      return 'Tổng giá trị - Phần chịu thuế'
    }
    return DEFAULT_TAX_PLACEHOLDER
  })

  const organizationOptions = ref([])
  const organizationSelectTree = ref(null)

  // Options (Mockup based on helpers/design)
  const typeOptions = [
    { label: 'Thông tin nhân viên', value: SalaryCompositionType.EmployeeInfo },
    { label: 'Chấm công', value: SalaryCompositionType.Timekeeping },
    { label: 'Doanh số', value: SalaryCompositionType.Sales },
    { label: 'KPI', value: SalaryCompositionType.KPI },
    { label: 'Sản phẩm', value: SalaryCompositionType.Production },
    { label: 'Lương', value: SalaryCompositionType.Salary },
    { label: 'Thuế TNCN', value: SalaryCompositionType.PersonalIncomeTax },
    { label: 'Bảo hiểm - Công đoàn', value: SalaryCompositionType.InsuranceAndUnion },
    { label: 'Khác', value: SalaryCompositionType.Other },
  ]
  const natureOptions = [
    { label: 'Thu nhập', value: SalaryCompositionNature.Income },
    { label: 'Khấu trừ', value: SalaryCompositionNature.Deduction },
    { label: 'Khác', value: SalaryCompositionNature.Other },
  ]
  const valueTypeOptions = [
    { label: 'Tiền tệ', value: ValueType.Currency },
    { label: 'Số', value: ValueType.Number },
    { label: 'Chữ', value: ValueType.Text },
    { label: 'Ngày', value: ValueType.Date },
    { label: 'Phần trăm', value: ValueType.Percentage },
  ]
  const taxStatusOptions = [
    { label: 'Chịu thuế', value: TaxStatus.Taxable },
    { label: 'Miễn thuế toàn phần', value: TaxStatus.FullyExempt },
    { label: 'Miễn thuế một phần', value: TaxStatus.PartiallyExempt },
  ]
  const aggregationOptions = [
    {
      label: 'Trong cùng đơn vị công tác',
      value: AggregationScope.SameOrganization,
      tooltip: 'Tự động tính bằng tổng giá trị của các nhân viên trong cùng đơn vị công tác',
    },
    {
      label: 'Dưới quyền',
      value: AggregationScope.Subordinate,
      tooltip:
        'Tự động tính bằng tổng giá trị của tất cả nhân viên dưới quyền (thuộc quyền quản lý trực tiếp hoặc gián tiếp)\n\n' +
        'Ví dụ: Quản lý kinh doanh A là quản lý trực tiếp của 3 nhân viên B, C, D. Giám đốc kinh doanh E là quản lý trực tiếp của A. Khi thiết lập:\n\n' +
        'Doanh số nhóm bằng tổng giá trị Doanh số cá nhân của tất cả nhân viên dưới quyền\n\n' +
        '+ Doanh số nhóm của Quản lý kinh doanh A = Doanh số cá nhân của B + Doanh số cá nhân của C + Doanh số cá nhân của D\n\n' +
        '+ Doanh số nhóm của Giám đốc kinh doanh E = Doanh số cá nhân của A + Doanh số cá nhân của B + Doanh số cá nhân của C + Doanh số cá nhân của D',
    },
    {
      label: 'Thuộc cơ cấu tổ chức',
      value: AggregationScope.Structure,
      tooltip:
        'Tự động tính bằng tổng giá trị của các nhân viên thuộc cơ cấu tổ chức (theo cấp thiết lập)',
    },
  ]
  const organizationLevelOptions = [
    { label: 'Cấp 1', value: 1 },
    { label: 'Cấp 2', value: 2 },
    { label: 'Cấp 3', value: 3 },
    { label: 'Cấp 4', value: 4 },
  ]
  const displayPayrollOptions = [
    { label: 'Có', value: DisplayOnPayroll.Yes },
    { label: 'Không', value: DisplayOnPayroll.No },
    { label: 'Chỉ nếu giá trị khác 0', value: DisplayOnPayroll.OnlyIfNonZero },
  ]
  const scStatusOptions = [
    { label: 'Đang theo dõi', value: 0 },
    { label: 'Ngừng theo dõi', value: 1 },
  ]
  const sourceOptions = [
    { label: 'Mặc định', value: SalaryCompositionSource.SystemDefault },
    { label: 'Tự thêm', value: SalaryCompositionSource.UserAdded },
  ]

  // Logic kiểm tra các loại đặc thù (Sản phẩm/KPI/Chấm công)
  const specialTypes = [
    SalaryCompositionType.Production,
    SalaryCompositionType.KPI,
    SalaryCompositionType.Timekeeping,
  ]

  // Kiểm tra xem có được phép dùng "Tự động cộng tổng" không
  const isAggregationDisabled = computed(() => {
    return [ValueType.Text, ValueType.Date, ValueType.Percentage].includes(form.sc_value_type)
  })

  // Watcher xử lý logic Loại thành phần đặc thù
  watch(
    () => form.sc_type,
    (newType) => {
      // Chặn logic mặc định khi đang load dữ liệu hoặc ở chế độ chỉ đọc (Sửa/Xem)
      if (isInitialLoading.value || isReadOnly.value) return

      if (specialTypes.includes(newType)) {
        setFieldValue('sc_nature', 2, { shouldValidate: false }) // Khác
        setFieldValue('sc_value_type', 1, { shouldValidate: false }) // Số
      }
    },
  )

  // Watcher xử lý logic Kiểu giá trị
  watch(
    () => form.sc_value_type,
    (newType) => {
      if (isInitialLoading.value || isReadOnly.value) return

      if ([ValueType.Text, ValueType.Date, ValueType.Percentage].includes(newType)) {
        setFieldValue('sc_calculation_method', 1, { shouldValidate: false }) // Ép về tính theo công thức
      }
    },
  )

  watch(
    () => form.sc_nature,
    (newNature) => {
      if (isInitialLoading.value || isReadOnly.value) return

      // Nếu là Thu nhập hoặc Khấu trừ, bắt buộc kiểu giá trị là Tiền tệ
      if (
        newNature === SalaryCompositionNature.Income ||
        newNature === SalaryCompositionNature.Deduction
      ) {
        setFieldValue('sc_value_type', ValueType.Currency, { shouldValidate: false })
      }

      // Reset trạng thái thuế khi không phải là Thu nhập
      if (newNature !== SalaryCompositionNature.Income) {
        setFieldValue('sc_tax_status', TaxStatus.Taxable, { shouldValidate: false })
        setFieldValue('sc_taxable_expression', '', { shouldValidate: false })
        setFieldValue('sc_exempt_expression', '', { shouldValidate: false })
      } else {
        setFieldValue('sc_tax_status', TaxStatus.Taxable, { shouldValidate: false }) // Default Chịu thuế nếu là Thu nhập
      }
    },
  )

  // Watcher xử lý khi thay đổi phương thức tính toán (Cộng tổng / Công thức)
  watch(
    () => form.sc_calculation_method,
    () => {
      // Nếu form đã từng nhấn Lưu (đang hiển thị lỗi), thì validate lại để xóa các lỗi
      // của phương thức cũ vừa chuyển đi, vì lúc này chúng không còn bị 'required' nữa.
      if (formSubmitted.value) {
        validateForm()
      }
    },
  )

  /**
   * Watcher xử lý sinh mã tự động từ Tên (Debounce 300ms)
   */
  let genCodeTimeout = null
  watch(
    () => form.sc_name,
    (newVal) => {
      // Nếu đang sửa bản ghi cũ, hoặc user đã sửa mã, hoặc tên rỗng thì không sinh mã tự động
      if (isEditMode.value || hasUserEditedCode.value || !newVal) return

      if (genCodeTimeout) clearTimeout(genCodeTimeout)
      genCodeTimeout = setTimeout(async () => {
        if (hasUserEditedCode.value) return
        try {
          isUpdatingCodeInternally.value = true
          const code = await salaryCompositionService.generateCode(newVal)

          // Nếu trong lúc đợi API, người dùng đã tự nhập mã thì bỏ qua kết quả API
          if (hasUserEditedCode.value) return

          setFieldValue('sc_code', code, { shouldValidate: false })

          // Đợi watcher của sc_code xử lý xong trước khi hạ cờ
          await nextTick()
          isUpdatingCodeInternally.value = false
        } catch (e) {
          console.error('Lỗi sinh mã:', e)
          isUpdatingCodeInternally.value = false
        }
      }, 300)
    },
  )

  /**
   * Computed property cho lỗi mã thành phần.
   * Ưu tiên hiển thị lỗi trùng mã (nếu có), sau đó mới đến lỗi từ VeeValidate (nếu form đã submit).
   */
  const codeError = computed(() => {
    if (duplicateCodeError.value) return duplicateCodeError.value
    return formSubmitted.value ? errors.value.sc_code : ''
  })

  /**
   * Watcher kiểm tra trùng mã khi người dùng nhập (Debounce 300ms).
   * Lỗi trùng mã được quản lý riêng biệt để hiển thị ngay lập tức.
   */
  let checkDupTimeout = null
  watch(
    () => form.sc_code,
    (newVal, oldVal) => {
      // Đánh dấu người dùng đã tự can thiệp vào mã
      if (!isUpdatingCodeInternally.value) {
        if (oldVal !== undefined && oldVal !== '' && !isReadOnly.value) {
          hasUserEditedCode.value = true
        }
      }

      if (!newVal || isReadOnly.value) return

      if (checkDupTimeout) clearTimeout(checkDupTimeout)
      checkDupTimeout = setTimeout(async () => {
        try {
          const isDuplicate = await salaryCompositionService.checkDuplicate(
            newVal,
            props.compositionId,
          )
          duplicateCodeError.value = isDuplicate ? 'Mã thành phần lương đã tồn tại' : null
          // Không gọi setErrors ở đây để tránh xung đột với VeeValidate và formSubmitted
          // VeeValidate sẽ tự kiểm tra required khi validateForm() được gọi
        } catch (e) {
          console.error('Lỗi check trùng mã:', e)
        }
      }, 300)
    },
  )

  /**
   * Lấy cây đơn vị công tác từ API.
   * Nếu là chế độ thêm mới (không nhân bản), tự động chọn toàn bộ đơn vị dưới node gốc.
   */
  async function fetchOrganizations() {
    try {
      const data = await organizationService.getOrganizationTree()
      if (data && Array.isArray(data)) {
        organizationOptions.value = data

        // Chỉ tự động chọn đơn vị mặc định nếu là Thêm mới thuần túy (không phải nhân bản)
        if (!isEditMode.value && !props.duplicateId && data.length > 0 && data[0]) {
          const rootOrgId = data[0].OrganizationId

          // Sử dụng setTimeout 0 hoặc nextTick để đảm bảo component con đã nhận options
          setTimeout(() => {
            // Để đồng bộ logic gom nhóm, ta nên lấy toàn bộ ID trong nhánh của root
            setFieldValue('organization_ids', getAllNodeIds(data[0]), { shouldValidate: false })
          }, 0)
        }
      }
    } catch (error) {
      showToast('Không thể tải danh sách đơn vị công tác', 'error')
    }
  }

  /**
   * Helper lấy toàn bộ ID của một node và các node con
   */
  function getAllNodeIds(node, ids = []) {
    ids.push(node.OrganizationId)
    if (node.Children && node.Children.length > 0) {
      node.Children.forEach((child) => getAllNodeIds(child, ids))
    }
    return ids
  }

  /**
   * Lấy danh sách thành phần lương phục vụ lookup trong công thức
   */
  const compositionOptions = ref([])
  async function fetchCompositionLookup() {
    try {
      const data = await salaryCompositionService.getLookup()
      if (data) {
        compositionOptions.value = data
      }
    } catch (error) {
      console.error('Không thể tải danh sách TPL cho công thức', error)
    }
  }

  /**
   * Tải dữ liệu chi tiết của thành phần lương khi ở chế độ Sửa/Xem.
   * Sau khi tải, gán dữ liệu vào VeeValidate và lưu bản sao vào originalForm.
   */
  async function fetchDataForEdit() {
    try {
      isInitialLoading.value = true
      const dto = await salaryCompositionService.getById(props.compositionId)
      if (dto) {
        // Map từ DTO (PascalCase) sang định dạng Form (snake_case)
        const editValues = {
          sc_name: dto.ScName,
          sc_code: dto.ScCode,
          organization_ids: dto.AppliedOrganizations?.map((o) => o.OrganizationId) || [],
          sc_type: dto.ScType,
          sc_nature: dto.ScNature,
          sc_is_tax_deductible: dto.IsTaxDeductible ? 1 : 0, // Chuyển đổi boolean sang 0/1
          sc_tax_status: dto.TaxStatus,
          sc_taxable_expression: dto.TaxableExpression,
          sc_exempt_expression: dto.ExemptExpression,
          sc_limit_expression: dto.LimitExpression,
          sc_allow_exceed_limit: dto.AllowExceedLimit ? 1 : 0, // Chuyển đổi boolean sang 0/1
          sc_value_type: dto.ValueType,
          sc_calculation_method: dto.CalculationMethod,
          sc_aggregation_scope: dto.AggregationScope,
          sc_organization_level: dto.OrganizationLevel,
          sc_composition_code: dto.CompositionCode,
          sc_formula_expression: dto.FormulaExpression,
          sc_description: dto.Description,
          sc_is_displayed_on_payroll: dto.IsDisplayedOnPayroll,
          sc_source: dto.ScSource,
          sc_status: dto.ScStatus,
        }
        resetVeeForm({ values: editValues })

        await nextTick()

        // Lưu lại bản gốc để phục vụ tính năng Hủy bỏ (Revert)
        originalForm.value = JSON.parse(JSON.stringify(form)) // Lưu bản sao để revert khi hủy
        isReadOnly.value = true
        hasUserEditedCode.value = false // Reset cờ khi load dữ liệu mới
      } else {
        showToast('Không tìm thấy thành phần lương', 'error')
        emit('close')
      }
    } catch (error) {
      showToast('Không thể tải dữ liệu thành phần lương', 'error')
      emit('close')
    } finally {
      isInitialLoading.value = false
    }
  }

  /**
   * Xử lý sự kiện Lưu dữ liệu.
   * Thực hiện validate, đơn giản hóa danh sách đơn vị (gom nhóm cha-con)
   * và gọi API tương ứng (Thêm mới hoặc Cập nhật).
   * @param {boolean} isSaveAndAdd - Nếu true, sau khi lưu sẽ không đóng form mà reset để thêm tiếp.
   */
  async function handleSave(isSaveAndAdd) {
    formSubmitted.value = true // Đánh dấu form đã được submit để hiển thị lỗi
    const { valid } = await validateForm() // Chạy validation schema
    if (!valid || duplicateCodeError.value) {
      showToast('Vui lòng kiểm tra lại các trường bị lỗi', 'error')
      return
    }

    try {
      if (isEditMode.value) {
        const submissionForm = { ...form }
        if (organizationSelectTree.value) {
          // Gọi hàm của SelectTree để lọc bỏ các ID con nếu đã chọn ID cha
          submissionForm.organization_ids = organizationSelectTree.value.getSimplifiedIds()
        }

        // Cập nhật thành phần lương hiện có
        await salaryCompositionService.update(props.compositionId, submissionForm)
        showToast('Cập nhật thành công')
        isReadOnly.value = true
        // Cập nhật lại originalForm sau khi lưu thành công
        originalForm.value = JSON.parse(JSON.stringify(form))
      } else {
        const submissionForm = { ...form }
        if (organizationSelectTree.value) {
          submissionForm.organization_ids = organizationSelectTree.value.getSimplifiedIds()
        }

        await salaryCompositionService.create(submissionForm)
        showToast('Thêm mới thành công')
        if (isSaveAndAdd) {
          resetForm() // Reset form để tiếp tục thêm mới
        } else {
          emit('close') // Đóng form và quay về danh sách
        }
      }
      emit('save') // Thông báo cho component cha để làm mới dữ liệu
      // Sau khi lưu, cần cập nhật lại lookup nếu có thay đổi tên/mã
      fetchCompositionLookup()
    } catch (error) {
      console.error('Lỗi khi lưu thành phần lương:', error)
      if (
        error.response &&
        error.response.status === 400 &&
        error.response.data &&
        error.response.data.data
      ) {
        // Xử lý lỗi validation trả về từ Server (ví dụ: trùng mã do Race Condition)
        const backendErrors = error.response.data.data
        const mappedErrors = {}
        for (const key in backendErrors) {
          if (Object.prototype.hasOwnProperty.call(backendErrors, key)) {
            const frontendKey = key.replace(/([A-Z])/g, '_$1').toLowerCase() // Chuyển đổi camelCase sang snake_case
            mappedErrors[frontendKey] = backendErrors[key]
          }
        }
        setErrors(mappedErrors)
        showToast('Vui lòng kiểm tra lại thông tin nhập', 'error')
      } else {
        showToast('Có lỗi xảy ra khi lưu thành phần lương', 'error')
      }
    }
  }

  /**
   * Reset toàn bộ form về trạng thái ban đầu và focus vào ô nhập liệu đầu tiên.
   */
  function resetForm() {
    resetVeeForm()
    hasUserEditedCode.value = false
    formSubmitted.value = false // Reset cờ hiển thị lỗi
    if (!isEditMode.value) {
      nextTick(() => {
        if (firstInputRef.value && typeof firstInputRef.value.focus === 'function') {
          firstInputRef.value.focus()
        } else if (firstInputRef.value?.$el?.querySelector('input')) {
          firstInputRef.value.$el.querySelector('input').focus()
        }
      })
    }
  }

  /**
   * Xử lý khi nhấn Hủy bỏ
   */
  function handleClose() {
    // Nếu đang ở chế độ nhập liệu và có thay đổi dữ liệu (dirty)
    if (!isReadOnly.value && meta.value.dirty) {
      pendingExitAction.value = 'close'
      isUnsavedChangesModalVisible.value = true
    } else {
      emit('close')
    }
  }

  /**
   * Hủy chỉnh sửa TPL
   */
  function handleCancelEdit() {
    if (isEditMode.value) {
      if (!isReadOnly.value) {
        if (meta.value.dirty) {
          pendingExitAction.value = 'revert'
          isUnsavedChangesModalVisible.value = true
          return
        }
        // Nếu đang sửa mà nhấn Hủy -> Revert dữ liệu về lúc mới mở form
        resetVeeForm({ values: originalForm.value })
        isReadOnly.value = true
        hasUserEditedCode.value = false
        formSubmitted.value = false
        return
      }
    }

    // Chế độ thêm mới hoặc đang xem
    handleClose()
  }

  /**
   * Xác nhận thoát mà không lưu dữ liệu
   */
  function confirmExit() {
    isUnsavedChangesModalVisible.value = false

    if (pendingExitAction.value === 'revert') {
      // Nếu là Hủy bỏ khi đang sửa: quay lại trạng thái chỉ đọc và khôi phục dữ liệu gốc
      resetVeeForm({ values: originalForm.value })
      isReadOnly.value = true
      hasUserEditedCode.value = false
      formSubmitted.value = false
    } else {
      // Nếu là Quay về: đóng form
      emit('close')
    }

    pendingExitAction.value = null
  }

  /**
   * Phát sự kiện nhân bản thành phần lương hiện tại.
   */
  function handleDuplicate() {
    emit('duplicate', props.compositionId)
  }

  /**
   * Gọi API xóa thành phần lương đang hiển thị trong form và đóng form.
   */
  async function confirmDelete() {
    try {
      // Thực hiện xóa (Logic kiểm tra bản ghi hệ thống đã được xử lý ở menu command)
      const deletedCount = await salaryCompositionService.bulkDelete([props.compositionId])
      if (deletedCount > 0) {
        showToast('Đã xóa thành phần lương')
        emit('save')
        emit('close')
      } else {
        showToast(`Không thể xóa thành phần lương <${form.sc_name ?? ''}>.`, 'error')
      }
    } catch (error) {
      console.error(error)
      showToast('Có lỗi xảy ra khi xóa thành phần lương', 'error')
    } finally {
      isDeleteModalVisible.value = false
    }
  }

  /**
   * Tải dữ liệu từ thành phần lương đích để điền vào form ở chế độ Nhân bản.
   * Chỉnh sửa tên thêm hậu tố "- Bản sao" và mở khóa form.
   */
  async function fetchDataForDuplicate() {
    try {
      isInitialLoading.value = true
      const dto = await salaryCompositionService.getById(props.duplicateId)
      if (dto) {
        const duplicateValues = {
          sc_name: dto.ScName + ' - Bản sao',
          organization_ids: dto.AppliedOrganizations?.map((o) => o.OrganizationId) || [],
          sc_type: dto.ScType,
          sc_nature: dto.ScNature,
          sc_is_tax_deductible: dto.IsTaxDeductible ? 1 : 0,
          sc_tax_status: dto.TaxStatus,
          sc_taxable_expression: dto.TaxableExpression,
          sc_exempt_expression: dto.ExemptExpression,
          sc_limit_expression: dto.LimitExpression,
          sc_allow_exceed_limit: dto.AllowExceedLimit ? 1 : 0,
          sc_value_type: dto.ValueType,
          sc_calculation_method: dto.CalculationMethod,
          sc_aggregation_scope: dto.AggregationScope,
          sc_organization_level: dto.OrganizationLevel,
          sc_composition_code: dto.CompositionCode,
          sc_formula_expression: dto.FormulaExpression,
          sc_description: dto.Description,
          sc_is_displayed_on_payroll: dto.IsDisplayedOnPayroll,
          sc_source: 1, // Nhân bản luôn là Tự thêm
          sc_status: 0, // Mặc định Đang theo dõi
        }

        resetVeeForm({ values: duplicateValues })

        await nextTick()
        isReadOnly.value = false // Mở khóa để nhập liệu
        hasUserEditedCode.value = false // Cho phép sinh mã tự động
        formSubmitted.value = false
      }
    } catch (error) {
      showToast('Không thể tải dữ liệu để nhân bản', 'error')
    } finally {
      isInitialLoading.value = false
    }
  }

  /**
   * Theo dõi sự thay đổi của props để fetch data (hỗ trợ component reuse khi nhân bản)
   */
  watch(
    () => [props.compositionId, props.duplicateId],
    ([newId, newDupId]) => {
      if (newId) {
        fetchDataForEdit()
      } else if (newDupId) {
        fetchDataForDuplicate()
      } else {
        resetForm()
      }
    },
    { immediate: true },
  )

  onMounted(() => {
    fetchOrganizations()
    fetchCompositionLookup()
  })

  return {
    form,
    isEditMode,
    isReadOnly,
    isDeleteModalVisible,
    formTitle,
    moreActions,
    nameError,
    orgIdsError,
    typeError,
    natureError,
    compCodeError,
    formulaExprError,
    taxableError,
    exemptError,
    codeError,
    isUnsavedChangesModalVisible,
    firstInputRef,
    organizationSelectTree,
    organizationOptions,
    compositionOptions,
    typeOptions,
    natureOptions,
    valueTypeOptions,
    taxStatusOptions,
    aggregationOptions,
    organizationLevelOptions,
    displayPayrollOptions,
    scStatusOptions,
    sourceOptions,
    isAggregationDisabled,
    isTaxableDisabled,
    isExemptDisabled,
    taxablePlaceholder,
    exemptPlaceholder,
    handleSave,
    handleCancelEdit,
    handleClose,
    handleDuplicate,
    confirmExit,
    confirmDelete,
    TaxStatus,
    SalaryCompositionSource,
  }
}
