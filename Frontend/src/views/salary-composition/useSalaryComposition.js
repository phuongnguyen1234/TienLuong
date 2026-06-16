import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from '@/utils/use-toast'
import salaryCompositionService from '@/services/salary-composition-service'
import {
  SalaryStatus,
  SalaryCompositionType,
  SalaryCompositionNature,
  TaxStatus,
  ValueType,
  SalaryCompositionSource,
  DisplayOnPayroll,
} from '@/models/SalaryEnums'
import {
  getTypeName,
  getNatureText,
  getTaxStatusText,
  getValueTypeText,
  getSourceText,
  getIsDisplayedOnPayrollText,
} from '@/utils/salary-composition-helpers.js'
import { FilterOperation } from '@/models/filter-operation.js'
import organizationService from '@/services/organization-service'
import { highlightFormula } from '@/utils/formula-highlighter'

export default function useSalaryComposition() {
  const route = useRoute()
  const router = useRouter() // Đối tượng router của Vue Router để điều hướng.

  // Dữ liệu chính của bảng thành phần lương
  // Mảng chứa các đối tượng thành phần lương được hiển thị trên bảng.
  const salaryCompositions = ref([])

  // Tổng số bản ghi thành phần lương từ API, dùng cho phân trang.
  const totalRecords = ref(0)

  // Trạng thái loading của bảng, dùng để hiển thị spinner hoặc skeleton.
  const isLoading = ref(false)

  // Biến lưu trữ từ khóa tìm kiếm nhập vào ô search.
  const searchQuery = ref('') // Expose

  // Biến lưu trữ từ khóa tìm kiếm sau khi đã debounce, dùng để kích hoạt tìm kiếm thực tế.
  const debouncedSearchQuery = ref('')

  // Trạng thái hiển thị của drawer lọc nâng cao.
  const isFilterDrawerOpen = ref(false)

  // Trạng thái hiển thị của modal thiết lập cột.
  const isTableSettingVisible = ref(false)

  // Vị trí hiển thị của modal thiết lập cột. // Expose
  const tableSettingPos = ref({ top: '0px', right: '32px' })

  // Trạng thái hiển thị của menu cột (khi click vào header cột).
  const isColumnMenuOpen = ref(false)

  // Cột hiện đang được click để mở menu (dùng cho sắp xếp, ghim cột).
  const activeFilterColumn = ref(null)

  // Element DOM của header cột đang được click.
  const activeHeaderEl = ref(null)

  // Vị trí hiển thị của menu cột.
  const columnMenuPosition = ref({ top: '0px', left: '0px' })

  // Trạng thái sắp xếp hiện tại của bảng (key: tên cột, order: 'asc'/'desc'/null).
  const currentSort = ref({ key: '', order: null }) // Expose

  // Danh sách các bộ lọc nâng cao đang được áp dụng từ FilterDrawer.
  const appliedFilters = ref([])

  // Trạng thái hiển thị của form thêm/sửa thành phần lương.
  const isFormVisible = ref(false)

  // Trạng thái hiển thị của modal xác nhận xóa.
  const isDeleteModalVisible = ref(false)

  // Trạng thái hiển thị của modal thông báo (ví dụ: không thể xóa bản ghi hệ thống) // Expose
  const isNotificationModalVisible = ref(false)
  // Nội dung thông báo hiển thị trên modal
  const notificationMessage = ref('')

  // Đối tượng thành phần lương đang được chọn để xóa.
  const deletingItem = ref(null)

  // Trạng thái modal xác nhận cập nhật trạng thái hàng loạt
  const isStatusConfirmVisible = ref(false)
  const statusConfirmMessage = ref('')
  const pendingStatus = ref(null)

  // ID của thành phần lương đang được sửa (nếu có).
  const editId = ref(null)

  // ID của thành phần lương đang được nhân bản (nếu có). // Expose
  const duplicateId = ref(null)

  // Trạng thái hiển thị của modal "Thêm từ danh mục hệ thống".
  const isAddFromSystemVisible = ref(false)

  // Trang hiện tại của bảng phân trang.
  const currentPage = ref(1)

  // Kích thước trang (số lượng bản ghi trên mỗi trang).
  const pageSize = ref(15) // Expose

  // Key dùng để lưu cấu hình cột vào LocalStorage.
  const GRID_KEY = 'pa_salary_composition'

  /**
   * Trả về cấu hình cột mặc định cho bảng thành phần lương
   * @returns {Array} Danh sách cấu hình các cột
   */
  const getDefaultColumns = () => [
    {
      key: 'ScCode',
      label: 'Mã thành phần',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: 'left',
      filterable: true,
      minWidth: 100,
    },
    {
      key: 'ScName',
      label: 'Tên thành phần',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: true,
      minWidth: 150,
    },
    {
      key: 'OrganizationNames',
      label: 'Đơn vị áp dụng',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: false,
      minWidth: 150,
    },
    {
      key: 'ScType',
      label: 'Loại thành phần',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: true,
      minWidth: 120,
    },
    {
      key: 'ScNature',
      label: 'Tính chất',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: true,
      minWidth: 100,
    },
    {
      key: 'TaxStatus',
      label: 'Chịu thuế',
      type: 'custom',
      width: '200px',
      visible: false,
      pinned: false,
      filterable: true,
      minWidth: 150,
    },
    {
      key: 'IsTaxDeductible',
      label: 'Giảm trừ khi tính thuế',
      type: 'custom',
      width: '200px',
      visible: false,
      pinned: false,
      filterable: true,
      minWidth: 150,
    },
    {
      key: 'LimitExpression',
      label: 'Định mức',
      type: 'custom',
      width: '200px',
      visible: false,
      pinned: false,
      filterable: true,
      minWidth: 150,
    },
    {
      key: 'ValueType',
      label: 'Kiểu giá trị',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: true,
      minWidth: 100,
    },
    {
      key: 'FormulaExpression',
      label: 'Giá trị',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: true,
      minWidth: 200,
    },
    {
      key: 'Description',
      label: 'Mô tả',
      type: 'custom',
      width: '200px',
      visible: false,
      pinned: false,
      filterable: true,
      minWidth: 200,
    },
    {
      key: 'ScSource',
      label: 'Nguồn tạo',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: true,
      minWidth: 120,
    },
    {
      key: 'IsDisplayedOnPayroll',
      label: 'Hiển thị trên phiếu lương',
      type: 'custom',
      width: '200px',
      visible: false,
      pinned: false,
      filterable: true,
      minWidth: 180,
    },

    {
      key: 'ScStatus',
      label: 'Trạng thái',
      type: 'custom',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: false,
      minWidth: 120,
    },
    {
      key: 'empty_spacer',
      label: '',
      width: '200px',
      visible: true,
      pinned: false,
      filterable: false,
      minWidth: 200,
      hideable: false,
    },
  ]

  /**
   * Fetch cấu hình cột từ DB và áp dụng. Nếu chưa có, lưu cấu hình mặc định vào DB.
   */
  const fetchGridConfig = async () => {
    const defaultConfig = getDefaultColumns()
    try {
      const response = await salaryCompositionService.getGridConfig(GRID_KEY)
      const savedJSON = response?.ConfigData

      if (savedJSON && savedJSON !== '[]') {
        const savedConfig = JSON.parse(savedJSON)
        const defaultConfigMap = new Map(defaultConfig.map((c) => [c.key, c]))

        // 1. Lấy các cột từ Database để giữ nguyên thứ tự (vị trí) đã lưu
        let finalConfig = savedConfig
          .filter((savedCol) => defaultConfigMap.has(savedCol.key))
          .map((savedCol) => {
            const defaultCol = defaultConfigMap.get(savedCol.key)
            let col = { ...defaultCol, ...savedCol }
            if (col.hideable === false) col.visible = true
            return col
          })

        // 2. Bổ sung các cột mới có trong code nhưng chưa có trong DB (phòng trường hợp cập nhật code)
        const savedKeys = new Set(savedConfig.map((c) => c.key))
        const newCols = defaultConfig.filter((c) => !savedKeys.has(c.key))
        finalConfig = [...finalConfig, ...newCols]

        // 3. Sắp xếp: Đưa cột ghim lên đầu, nhưng vẫn giữ thứ tự tương đối của chúng
        finalConfig.sort((a, b) => {
          if (a.key === 'empty_spacer') return 1
          if (b.key === 'empty_spacer') return -1
          const aP = a.pinned === 'left' ? 1 : 0
          const bP = b.pinned === 'left' ? 1 : 0
          return bP - aP
        })

        columnsConfig.value = finalConfig
      } else {
        // Nếu chưa có cấu hình trong DB, thực hiện lưu cấu hình mặc định
        await salaryCompositionService.saveGridConfig(GRID_KEY, defaultConfig)
        columnsConfig.value = defaultConfig
      }
    } catch (e) {
      console.error('Lỗi khi tải hoặc lưu cấu hình cột từ DB:', e)
      // Fallback về mặc định nếu có lỗi xảy ra
      columnsConfig.value = defaultConfig
    }
  }

  const columnsConfig = ref(getDefaultColumns())

  // Cấu hình danh sách các cột có thể lọc cho FilterDrawer
  const filterDrawerColumns = computed(() => {
    // Định nghĩa mapping giữa Key của cột và các Options tương ứng từ Enum
    const ENUM_COLUMN_MAP = {
      ScType: Object.values(SalaryCompositionType).map((v) => ({
        label: getTypeName(v),
        value: v,
      })),
      ScNature: Object.values(SalaryCompositionNature).map((v) => ({
        label: getNatureText(v),
        value: v,
      })),
      TaxStatus: Object.values(TaxStatus).map((v) => ({ label: getTaxStatusText(v), value: v })),
      IsTaxDeductible: [
        { label: 'Có', value: 1 },
        { label: 'Không', value: 0 },
      ],
      ValueType: Object.values(ValueType).map((v) => ({ label: getValueTypeText(v), value: v })),
      ScSource: Object.values(SalaryCompositionSource).map((v) => ({
        label: getSourceText(v),
        value: v,
      })),
      IsDisplayedOnPayroll: Object.values(DisplayOnPayroll).map((v) => ({
        label: getIsDisplayedOnPayrollText(v),
        value: v,
      })),
    }

    return getDefaultColumns()
      .filter((col) => col.label && col.filterable !== false)
      .map((col) => {
        const options = ENUM_COLUMN_MAP[col.key]

        return {
          key: col.key,
          label: col.label,
          type: options ? 'enum' : col.type === 'custom' ? 'text' : col.type || 'text',
          options: options || [],
        }
      })
  })

  const visibleColumns = computed(() => {
    // Expose
    return columnsConfig.value
      .filter((col) => col.visible !== false)
      .sort((a, b) => {
        const aPinned = a.pinned === 'left'
        const bPinned = b.pinned === 'left'
        if (aPinned === bPinned) return 0
        return aPinned ? -1 : 1
      })
  })
  const selectedIds = ref([]) // Expose

  const { showToast } = useToast()

  // Các mục trong dropdown của nút "Thêm"
  // Bao gồm tùy chọn "Chọn từ danh mục của hệ thống" để mở modal AddFromSystem.
  // @type {Array<Object>}
  const addDropdownItems = [
    {
      label: 'Chọn từ danh mục của hệ thống',
      command: () => {
        isAddFromSystemVisible.value = true
      },
    },
  ] // Expose

  // Các tùy chọn cho bộ lọc trạng thái (Đang theo dõi, Ngừng theo dõi, Tất cả).
  // @type {Array<Object>}
  const statusOptions = [
    { label: 'Tất cả', value: 'all' },
    { label: 'Đang theo dõi', value: 'active' },
    { label: 'Ngừng theo dõi', value: 'inactive' },
  ]
  // Expose
  // Trạng thái được chọn hiện tại trong bộ lọc trạng thái.
  const selectedStatus = ref('all')

  // Danh sách các ID đơn vị được chọn từ SelectTree để lọc.
  const selectedOrganizations = ref([])
  // Expose
  // Tên của đơn vị gốc (cấp 0), thường là "Tổng công ty", dùng để hiển thị mặc định.
  const organizationOptions = ref([])

  // Danh sách các đơn vị công tác dưới dạng cây, dùng cho SelectTree.
  // @type {Array<Object>}
  const rootOrganizationName = ref('Tất cả đơn vị')

  /**
   * Mở form thêm mới thành phần lương.
   * Đặt `editId` và `duplicateId` về null để đảm bảo là chế độ thêm mới.
   */
  const openModal = () => {
    // Expose
    editId.value = null
    duplicateId.value = null

    isFormVisible.value = true
  }

  /**
   * Trả về danh sách tất cả các ID đơn vị đã được chọn từ SelectTree.
   * SelectTree (thông qua TreeView) sẽ trả về tất cả các ID của các node được chọn,
   * bao gồm cả node cha và con nếu chúng được chọn.
   */
  const getSelectedOrganizationIdsForFilter = () => {
    return selectedOrganizations.value
  }

  /**
   * Lấy danh sách đơn vị công tác từ API và xây dựng cấu trúc cây.
   * Bỏ qua cấp 0 (Tổng công ty) nếu có, và lưu tên đơn vị gốc.
   * Hiển thị toast lỗi nếu không thể tải dữ liệu.
   * Lấy danh sách đơn vị công tác và bỏ qua cấp 0 (Tổng công ty)
   */
  async function fetchOrganizations() {
    try {
      const data = await organizationService.getOrganizationTree()
      if (data && Array.isArray(data)) {
        // Lưu lại tên của đơn vị cấp 0 (thường là phần tử đầu tiên trong mảng tree)
        if (data.length > 0) {
          rootOrganizationName.value = data[0].OrganizationName
        }
        // Gán toàn bộ cây đơn vị (trừ node gốc nếu có) cho SelectTree
        organizationOptions.value = data.length > 0 ? data[0].Children || [] : []
      }
    } catch (error) {
      showToast('Không thể tải danh sách đơn vị công tác', 'error')
    }
  }

  /**
   * Xử lý sự kiện chỉnh sửa một thành phần lương.
   * Đặt `editId` là ID của thành phần lương cần sửa và hiển thị form.
   * @param {Object} item - Đối tượng thành phần lương cần chỉnh sửa.
   */
  const handleEdit = (item) => {
    // Expose
    editId.value = item.ScId
    duplicateId.value = null
    isFormVisible.value = true
  }

  const handleDelete = (item) => {
    // Expose
    if (item.ScSource === SalaryCompositionSource.SystemDefault) {
      notificationMessage.value =
        'Đây là thành phần lương mặc định của hệ thống nên không thể xóa. Vui lòng kiểm tra lại.'
      isNotificationModalVisible.value = true
      return
    }
    deletingItem.value = item
    isDeleteModalVisible.value = true
  }

  const handleConfirmAddFromSystem = (items) => {
    // Expose
    console.log('Các mục đã chọn từ hệ thống:', items)
    showToast(`Đã thêm thành công ${items.length} thành phần từ hệ thống`)
  }

  const deleteConfirmMessage = computed(() => {
    if (deletingItem.value) {
      return `Bạn có chắc chắn muốn xóa thành phần lương <${deletingItem.value.ScName}> không?`
    }
    return `Bạn có chắc chắn muốn xóa các thành phần lương đã chọn không?`
  })

  /**
   * Xác nhận xóa (xử lý chung cho xóa đơn và xóa hàng loạt)
   */
  const confirmDelete = async () => {
    // Expose
    try {
      let idsToDelete = []
      let initialCount = 0

      if (deletingItem.value) {
        // Trường hợp xóa một dòng từ row action hoặc từ form
        idsToDelete = [deletingItem.value.ScId]
        initialCount = 1
      } else {
        // Trường hợp xóa hàng loạt khi đã tích chọn
        idsToDelete = selectedIds.value
        initialCount = idsToDelete.length
      }

      if (idsToDelete.length === 0) return

      const response = await salaryCompositionService.bulkDelete(idsToDelete)

      // Lấy số lượng bản ghi đã xóa từ thuộc tính Data
      const deletedCount = response

      if (deletedCount > 0) {
        if (deletingItem.value) {
          showToast('Đã xóa thành phần lương')
        } else {
          if (deletedCount < initialCount) {
            showToast(
              'Đã xóa thành phần lương, trong đó có thành phần lương hệ thống không thể bị xóa',
            )
          } else {
            showToast('Đã xóa thành phần lương')
          }
        }
        fetchData()
        selectedIds.value = []
      }
    } catch (error) {
      console.error('Error during delete operation:', error) // Thêm dòng này để kiểm tra lỗi
      showToast('Có lỗi xảy ra khi xóa dữ liệu', 'error')
    } finally {
      isDeleteModalVisible.value = false
      deletingItem.value = null
    }
  }

  // Computed property để lấy danh sách các thành phần lương đang được chọn.
  // @type {ComputedRef<Array<Object>>}
  const selectedSalaryCompositions = computed(() => {
    return salaryCompositions.value.filter((comp) => selectedIds.value.includes(comp.ScId))
  })

  // Computed property để xác định có nên hiển thị nút "Ngừng theo dõi hàng loạt" hay không.
  // Nút này hiển thị nếu có ít nhất một thành phần lương đang ở trạng thái "Đang theo dõi" trong các mục đã chọn.
  // @type {ComputedRef<boolean>}
  const showBulkStopTrackingButton = computed(() => {
    return selectedSalaryCompositions.value.some((comp) => comp.ScStatus === SalaryStatus.Active)
  })

  // Computed property để xác định có nên hiển thị nút "Đang theo dõi hàng loạt" hay không.
  // Nút này hiển thị nếu có ít nhất một thành phần lương đang ở trạng thái "Ngừng theo dõi" trong các mục đã chọn.
  // @type {ComputedRef<boolean>}
  const showBulkStartTrackingButton = computed(() => {
    return selectedSalaryCompositions.value.some((comp) => comp.ScStatus === SalaryStatus.Inactive)
  })

  /**
   * Cập nhật trạng thái hàng loạt
   * @param {number} status - 0: Đang theo dõi, 1: Ngừng theo dõi
   * Gọi API để cập nhật trạng thái cho các thành phần lương đã chọn và làm mới dữ liệu.
   */
  const handleBulkUpdateStatus = (status) => {
    pendingStatus.value = status
    statusConfirmMessage.value =
      status === 0
        ? 'Bạn có chắc chắn muốn tiếp tục theo dõi các thành phần lương đã chọn không?'
        : 'Bạn có chắc chắn muốn ngừng theo dõi các thành phần lương đã chọn không?'
    isStatusConfirmVisible.value = true
  }

  /**
   * Xác nhận cập nhật trạng thái sau khi nhấn đồng ý trên modal
   */
  const confirmBulkUpdateStatus = async () => {
    // Expose
    try {
      // 1. Gọi service để cập nhật trạng thái hàng loạt.
      const response = await salaryCompositionService.bulkUpdateStatus(
        selectedIds.value,
        pendingStatus.value,
      )
      const isSuccess = response?.IsSuccess || response === true

      if (isSuccess) {
        showToast(
          pendingStatus.value === 0
            ? 'Đã tiếp tục theo dõi thành công'
            : 'Đã ngừng theo dõi thành công',
        )
        fetchData()
        selectedIds.value = []
      }
    } catch (error) {
      showToast('Có lỗi xảy ra khi cập nhật trạng thái', 'error')
    } finally {
      isStatusConfirmVisible.value = false
    }
  }

  /**
   * Xử lý khi ấn nút Xóa hàng loạt trên Action Bar
   */
  const handleBulkDelete = () => {
    // Expose
    const selectedComps = selectedSalaryCompositions.value
    const hasUserRecords = selectedComps.some(
      (c) => c.ScSource !== SalaryCompositionSource.SystemDefault,
    )
    const hasSystemRecords = selectedComps.some((c) => c.ScSource === 0)

    if (!hasUserRecords && hasSystemRecords) {
      notificationMessage.value =
        'Các thành phần lương đã chọn đều là mặc định của hệ thống nên không thể xóa. Vui lòng kiểm tra lại.'
      isNotificationModalVisible.value = true
      return
    }

    deletingItem.value = null // Null để confirmDelete biết là xóa nhiều
    isDeleteModalVisible.value = true
  }

  const handleDuplicate = (item) => {
    // Expose
    editId.value = null
    duplicateId.value = item.ScId
    isFormVisible.value = true
  }
  /**
   * Xử lý nhân bản từ bên trong form (khi người dùng click nút "Nhân bản" trong form).
   * Đặt `editId` về null và `duplicateId` là ID của mục cần nhân bản. // Expose
   * @param {string} id - ID của thành phần lương cần nhân bản.
   */
  const handleDuplicateFromForm = (id) => {
    editId.value = null
    duplicateId.value = id
  }

  /**
   * Xử lý hiển thị thông báo lỗi/cảnh báo từ Form
   * @param {string} message - Nội dung thông báo
   */
  const handleFormNotification = (message) => {
    notificationMessage.value = message
    isNotificationModalVisible.value = true
  }

  /**
   * Chuyển đổi trạng thái "Đang theo dõi" / "Ngừng theo dõi" cho một thành phần lương.
   * @param {Object} item - Đối tượng thành phần lương cần thay đổi trạng thái.
   */
  const handleToggleStatus = async (item) => {
    // Expose
    const newStatus =
      item.ScStatus === SalaryStatus.Active ? SalaryStatus.Inactive : SalaryStatus.Active
    try {
      await salaryCompositionService.update(item.ScId, { ...item, ScStatus: newStatus })
      showToast(`Đã thay đổi trạng thái theo dõi cho: ${item.ScName}`)
      fetchData()
    } catch (error) {
      showToast('Không thể cập nhật trạng thái', 'error')
    }
  }

  /**
   * Hàm chính để lấy dữ liệu từ API dựa trên paging, search và filter
   * Xây dựng các tham số tìm kiếm và lọc, sau đó gọi API và cập nhật dữ liệu bảng.
   * Hiển thị toast lỗi nếu có vấn đề khi tải dữ liệu.
   */
  async function fetchData() {
    // Expose
    isLoading.value = true
    try {
      // 1. Khởi tạo mảng filters để chứa các tiêu chí lọc.
      const filters = []
      // 1. Map lọc trạng thái từ select Label
      if (selectedStatus.value !== 'all') {
        const statusVal = selectedStatus.value === 'active' ? 0 : 1
        filters.push(`ScStatus:${FilterOperation.Equals}:${statusVal}`)
      }

      // 2. Lọc theo đơn vị công tác (nếu có chọn trên SelectTree)
      const filterOrgIds = getSelectedOrganizationIdsForFilter()
      if (filterOrgIds.length > 0) {
        // Theo yêu cầu: không dùng 'in', lọc theo đơn vị thì bằng luôn value.
        // Nếu có nhiều nhánh độc lập được chọn, chúng ta gửi các filter Equals cho từng ID.
        filterOrgIds.forEach((id) => {
          filters.push(`OrganizationId:${FilterOperation.Equals}:${id}`)
        })
      }

      // 3. Thêm các bộ lọc nâng cao từ FilterDrawer vào mảng filters.
      // 3. Map các lọc nâng cao từ Drawer
      appliedFilters.value.forEach((f) => {
        if (f.value !== null && f.value !== undefined && f.value !== '') {
          // Nếu là mảng (multi-select), tách thành nhiều filter cùng key
          if (Array.isArray(f.value)) {
            if (f.value.length > 0) {
              f.value.forEach((val) => {
                filters.push(`${f.key}:${f.operator}:${val}`)
              })
            }
          } else {
            filters.push(`${f.key}:${f.operator}:${f.value}`)
          }
        }
      })

      // 4. Gọi API để lấy dữ liệu phân trang.
      const result = await salaryCompositionService.getPaging(
        debouncedSearchQuery.value,
        filters,
        currentPage.value,
        pageSize.value,
      )

      // 5. Cập nhật dữ liệu bảng và tổng số bản ghi.
      salaryCompositions.value = result.Items || []
      totalRecords.value = result.TotalItems || 0
      // 6. Xử lý lỗi nếu API trả về lỗi.
    } catch (error) {
      showToast('Không thể tải dữ liệu thành phần lương', 'error')
    } finally {
      isLoading.value = false
    }
  }

  // Timeout cho debounce search để tránh gọi API quá nhiều lần khi người dùng đang gõ.
  let searchTimeout = null
  watch(searchQuery, (newVal) => {
    if (searchTimeout) clearTimeout(searchTimeout)
    searchTimeout = setTimeout(() => {
      debouncedSearchQuery.value = newVal
    }, 300)
  })

  // Theo dõi các thay đổi của các biến liên quan đến phân trang, tìm kiếm và lọc để gọi lại API.
  watch(
    [
      debouncedSearchQuery,
      currentPage,
      pageSize,
      selectedStatus,
      selectedOrganizations,
      appliedFilters,
    ],
    () => fetchData(),
    { deep: true },
  )

  /**
   * Xử lý khi các bộ lọc nâng cao từ FilterDrawer được áp dụng.
   * Cập nhật biến `appliedFilters` và kích hoạt tải lại dữ liệu.
   * @param {Array<Object>} filters - Danh sách các bộ lọc đã áp dụng.
   */
  function handleFilterApply(filters) {
    // Expose
    appliedFilters.value = filters
  }

  /**
   * Mở modal thiết lập cột và tính toán vị trí hiển thị.
   * @param {Event} event - Sự kiện click từ nút "Thiết lập bảng".
   */
  function handleOpenTableSetting(event) {
    // Expose
    const rect = event.currentTarget.getBoundingClientRect()
    tableSettingPos.value = {
      top: `${rect.bottom + 8}px`,
      right: '32px',
      bottom: '70px',
    }
    isTableSettingVisible.value = true
  }

  /**
   * Lưu cấu hình cột mới vào LocalStorage và cập nhật lại `columnsConfig`.
   * Hiển thị toast thông báo thành công.
   * @param {Array<Object>} newSettings - Cấu hình cột mới.
   */
  async function handleColumnSettingsSave(newSettings) {
    try {
      await salaryCompositionService.saveGridConfig(GRID_KEY, newSettings)
      columnsConfig.value = newSettings
      showToast('Lưu thiết lập bảng thành công')
      isTableSettingVisible.value = false
    } catch (e) {
      showToast('Không thể lưu thiết lập bảng', 'error')
    }
  }

  /**
   * Khôi phục cấu hình cột về mặc định, xóa cài đặt từ LocalStorage.
   * Cập nhật lại `columnsConfig` và hiển thị toast thông báo.
   * Đảm bảo các cột không thể ẩn vẫn hiển thị.
   */
  async function handleColumnSettingsReset() {
    try {
      const defaults = getDefaultColumns()
      await salaryCompositionService.saveGridConfig(GRID_KEY, defaults)
      columnsConfig.value = defaults
      showToast('Đã khôi phục thiết lập mặc định')
    } catch (e) {
      showToast('Không thể khôi phục thiết lập', 'error')
    }
  }

  /**
   * Cập nhật vị trí của ColumnMenu dựa trên vị trí của element kích hoạt.
   * Tính toán vị trí hiển thị của ColumnMenu
   */
  function updateColumnMenuPosition(el) {
    if (!el) return
    const MENU_WIDTH = 200
    const targetRect = el.getBoundingClientRect()

    const top = targetRect.bottom + 2
    let left = targetRect.left + 12

    // Tránh tràn lề phải màn hình
    if (left + MENU_WIDTH > window.innerWidth) {
      left = window.innerWidth - MENU_WIDTH - 16
    }

    columnMenuPosition.value = {
      top: `${top}px`,
      left: `${left}px`,
    }
  }

  /**
   * Xử lý sự kiện click vào header cột để mở ColumnMenu.
   * Lưu element header, cột được click và tính toán vị trí menu.
   * Mở menu khi click vào header
   * @param {Object} column - Đối tượng cột được click.
   * @param {Event} event - Sự kiện click.
   */
  function handleHeaderClick(column, event) {
    // Expose
    activeHeaderEl.value = event.currentTarget
    updateColumnMenuPosition(activeHeaderEl.value)
    activeFilterColumn.value = column
    isColumnMenuOpen.value = true
  }

  /** // Expose
   * Xử lý sự kiện sắp xếp cột.
   * Cập nhật `currentSort` với key và thứ tự sắp xếp mới.
   * @param {string} key - Key của cột được sắp xếp.
   * @param {string|null} order - Thứ tự sắp xếp ('asc', 'desc', hoặc null để bỏ sắp xếp).
   */
  function handleSort(key, order) {
    // Expose
    currentSort.value = { key, order }
  }

  /**
   * Xử lý sự kiện ghim/bỏ ghim cột. // Expose
   */
  async function handleTogglePin(column) {
    const col = columnsConfig.value.find((c) => c.key === column.key)
    if (col) {
      if (col.pinned) {
        col.pinned = false
      } else {
        const totalPinned = columnsConfig.value.filter((c) => c.pinned).length
        if (totalPinned >= 5) {
          showToast('Không thể ghim quá 5 cột', 'warning')
          return
        }
        col.pinned = 'left'
      }
      // Lưu vào DB ngay khi ghim
      await salaryCompositionService.saveGridConfig(GRID_KEY, columnsConfig.value)
      // Buộc Table render lại vị trí sticky
      columnsConfig.value = [...columnsConfig.value]
      showToast(`Đã ${col.pinned ? 'ghim' : 'bỏ ghim'} cột ${col.label}`)
    }
  }

  const handleResize = () => {
    if (isColumnMenuOpen.value) updateColumnMenuPosition(activeHeaderEl.value)
  }

  onMounted(() => {
    // Load cấu hình cột
    fetchGridConfig()
    // gọi API lấy dữ liệu TPL
    fetchData()

    // Gọi API để lấy dữ liệu đơn vị
    fetchOrganizations()
    window.addEventListener('resize', handleResize)
  })

  onUnmounted(() => {
    window.removeEventListener('resize', handleResize)
  })

  return {
    salaryCompositions,
    totalRecords,
    isLoading,
    searchQuery,
    isFilterDrawerOpen,
    isTableSettingVisible,
    tableSettingPos,
    isColumnMenuOpen,
    activeFilterColumn,
    columnMenuPosition,
    currentSort,
    appliedFilters,
    isFormVisible,
    isDeleteModalVisible,
    isNotificationModalVisible,
    isStatusConfirmVisible,
    statusConfirmMessage,
    notificationMessage,
    editId,
    duplicateId,
    isAddFromSystemVisible,
    currentPage,
    pageSize,
    columnsConfig,
    visibleColumns,
    selectedIds,
    addDropdownItems,
    statusOptions,
    selectedStatus,
    selectedOrganizations,
    organizationOptions,
    rootOrganizationName,
    filterDrawerColumns,
    deleteConfirmMessage,
    showBulkStopTrackingButton,
    showBulkStartTrackingButton,
    openModal,
    handleEdit,
    handleDelete,
    handleConfirmAddFromSystem,
    confirmDelete,
    handleBulkUpdateStatus,
    confirmBulkUpdateStatus,
    handleBulkDelete,
    handleDuplicate,
    handleDuplicateFromForm,
    handleFormNotification,
    handleToggleStatus,
    fetchData,
    handleFilterApply,
    handleOpenTableSetting,
    handleColumnSettingsSave,
    handleColumnSettingsReset,
    handleHeaderClick,
    handleSort,
    handleTogglePin,
    highlightFormula,
  }
}
