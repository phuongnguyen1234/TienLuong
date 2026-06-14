<template>
  <!--sidebar-->
  <div class="sidebar sidebar_active" :class="{ sidebar_expanded: isExpanded }">
    <!--sidebar menu-->
    <div class="sidebar_menu">
      <Tooltip
        v-for="item in menuItems"
        :key="item.name"
        :text="!isExpanded && !item.hasDropdown ? item.name : ''"
        position="right"
        class="sidebar_tooltip_wrapper"
      >
        <div
          class="sidebar_menu_item"
          :class="{
            active: route.matched.some((r) => r.name === item.route),
            'has-submenu': item.hasDropdown,
          }"
          @click="router.push({ name: item.route })"
          @mouseenter="handleMouseEnter($event, item)"
        >
          <div class="sidebar_menu_icon">
            <svg
              width="20"
              height="20"
              viewBox="0 0 20 20"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
              v-html="item.iconSvg"
            ></svg>
          </div>
          <div class="menu_text">{{ item.name }}</div>
          <div v-if="item.hasDropdown" class="dropdown-arrow">
            <svg
              width="16"
              height="16"
              viewBox="0 0 16 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M6 12L10 8L6 4"
                stroke="currentColor"
                stroke-width="1.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </div>

          <!-- Submenu (Flyout menu) -->
          <div
            v-if="item.hasDropdown"
            class="submenu"
            :style="{ top: submenuTop + 'px', left: submenuLeft + 'px' }"
          >
            <div
              v-for="sub in item.dropdownItems"
              :key="sub.name"
              class="submenu_item"
              :class="{ active: route.matched.some((r) => r.name === sub.route) }"
              @click.stop="router.push({ name: sub.route })"
            >
              {{ sub.name }}
            </div>
          </div>
        </div>
      </Tooltip>
    </div>

    <!--sidebar toggle button-->
    <button class="sidebar_toggle" @click="emit('toggle')">
      <svg
        width="16"
        height="16"
        viewBox="0 0 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M10.8333 5.83334H17.5M10.8333 14.1667H17.5M14.1667 10H17.5M2.5 10H10.8333M2.5 10L6.25 13.75M2.5 10L6.25 6.25001"
          stroke="currentColor"
          stroke-width="1.5"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </button>
  </div>
</template>

<style scoped>
.sidebar {
  position: absolute;
  top: 48px;
  width: var(--sidebar-width-collapsed);
  bottom: 0;
  left: 0;
  padding: 15px; /* Giữ nguyên padding 15px theo yêu cầu */
  background-color: white; /* Updated background */
  /* Removed background-image and background-size: cover */
  box-sizing: border-box;
  display: none;
  transition: width 0.3s ease;
  z-index: 100;
  border-right: 1px solid var(--border-control-normal);
}

.sidebar_active {
  display: flex !important;
  flex-direction: column;
}

.sidebar_expanded {
  width: var(--sidebar-width-expanded);
}

/* Removed .sidebar_overlay */

.sidebar_menu {
  flex: 1;
  overflow-y: auto; /* Allow vertical scroll */
  position: relative;
  z-index: 1; /* Keep z-index for stacking context */
  display: flex; /* Added flex for gap */
  flex-direction: column; /* Added flex for gap */
  gap: 4px; /* Added gap between items */
  overflow-x: hidden; /* Prevent horizontal scroll */
}

.sidebar_tooltip_wrapper {
  display: block !important;
  width: 100%;
}

/* Custom Scrollbar for sidebar menu */
.sidebar_menu::-webkit-scrollbar {
  width: var(--scrollbar-size);
}

.sidebar_menu::-webkit-scrollbar-track {
  background: transparent;
}

.sidebar_menu::-webkit-scrollbar-thumb {
  background: var(--color-scrollbar);
  border-radius: 4px;
}

.sidebar_menu::-webkit-scrollbar-thumb:hover {
  background: var(--color-scrollbar-hover);
}

.sidebar_menu_item {
  padding: 8px 5px; /* Khi collapse: (30px vùng chứa - 20px icon) / 2 = 5px để icon nằm chính giữa */
  display: flex;
  align-items: center;
  margin: 0; /* Removed margin to use gap on parent */
  color: var(--color-text-main); /* Normal text color */
  background-color: white; /* Normal background color */
  border-radius: 8px; /* From button design */
  cursor: pointer;
  position: relative; /* For dropdown positioning */
}

.sidebar_menu_item:hover {
  background-color: var(--item-hover); /* Hover background color */
}

.sidebar_expanded .sidebar_menu_item {
  padding: 8px 15px; /* Tăng padding khi mở rộng để giao diện thoáng và cân đối với text */
}

.sidebar_menu_item:hover .submenu {
  visibility: visible;
  opacity: 1;
}

.submenu {
  visibility: hidden;
  opacity: 0;
  position: fixed; /* Fix: Thoát khỏi overflow của container cuộn */
  z-index: calc(var(--z-index-dropdown) + 100);
  background-color: var(--bg-white);
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  border-radius: var(--border-radius-base);
  padding: 16px;
  border: 1px solid var(--border-control-normal);
  min-width: 180px;
  display: flex;
  flex-direction: column;
  gap: 2px;
  cursor: default;
  transition: opacity 0.2s ease;
}

/* Lớp đệm "Bridge" để giữ trạng thái hover khi di chuyển chuột qua khoảng trống 16px + padding */
.submenu::before {
  content: '';
  position: absolute;
  left: -40px; /* Mở rộng đủ để che phủ 16px gap + 15px padding sidebar + buffer */
  top: -10px; /* Mở rộng thêm một chút chiều dọc để tránh trượt chuột */
  bottom: -10px;
  width: 40px;
}

.submenu_item {
  padding: 8px 15px; /* Đồng nhất với sidebar item */
  color: var(--color-text-main);
  font-size: 13px; /* Đồng nhất với sidebar item */
  font-weight: 500;
  border-radius: 8px; /* Đồng nhất với sidebar item */
  white-space: nowrap;
  transition: all 0.2s ease;
  cursor: pointer;
}

.submenu_item:hover {
  background-color: var(--item-hover); /* Style hover giống sidebar */
}

.submenu_item.active {
  background-color: #eaf7ee; /* Màu focus từ Redesign */
  color: #34b057; /* Màu primary từ Redesign */
}

.sidebar_menu_item.active {
  background-color: var(--bg-sidebar-selected); /* Selected background color */
  color: var(--color-primary); /* Selected text color */
}

.sidebar_menu_item.active .menu_text {
  color: var(--color-primary); /* Selected text color */
}

.sidebar_menu_icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px; /* Icon size 20x20 */
  height: 20px; /* Icon size 20x20 */
  flex-shrink: 0; /* Prevent icon from shrinking */
}

/* Style for SVG icons */
.sidebar_menu_icon svg {
  width: 100%;
  height: 100%;
  color: var(--color-icon); /* Default icon color */
}

.sidebar_menu_item.active .sidebar_menu_icon svg {
  color: var(--color-primary); /* Active icon color */
}

.menu_text {
  display: none;
  color: var(--color-text-main); /* Default text color */
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
  flex-shrink: 0;
  padding-left: 8px;
  flex-grow: 1; /* Allow text to take available space */
}

.sidebar_expanded .menu_text {
  display: block;
}

.dropdown-arrow {
  display: none; /* Hidden by default */
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  flex-shrink: 0;
  margin-left: auto; /* Push to the right */
}

.sidebar_expanded .dropdown-arrow {
  display: flex; /* Show arrow when expanded */
}

.dropdown-arrow svg {
  color: var(--color-icon); /* Default arrow color */
}

.sidebar_menu_item.active .dropdown-arrow svg {
  color: var(--color-primary); /* Active arrow color */
}

.sidebar_toggle {
  position: absolute;
  bottom: 0;
  right: 0;
  height: 40px;
  border: 1px solid var(--border-control-normal);
  border-bottom: none;
  border-right: none;
  margin: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  background-color: transparent;
  transition: all 0.3s ease;
  flex-shrink: 0;
  color: var(--color-icon);
  cursor: pointer;
  z-index: 10;
}

/* Sidebar đóng (Collapsed) */
.sidebar_toggle {
  left: 0;
  right: 0;
  width: 100%;
  border-left: none;
  border-top-left-radius: 0;
}

.sidebar_toggle svg {
  transform: scaleX(-1); /* Flip icon khi đóng */
}

.sidebar_expanded .sidebar_toggle {
  left: auto;
  right: 0;
  width: 40px;
  border-left: 1px solid var(--border-control-normal);
  border-top-left-radius: 8px; /* Bo góc trên trái 8px khi mở */
}

.sidebar_expanded .sidebar_toggle svg {
  transform: scaleX(1);
}

.sidebar_expanded .sidebar_toggle_text {
  display: block;
  animation: fadeIn 0.3s ease;
}

.sidebar_toggle:hover {
  background-color: var(--item-hover);
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}
/*end of style sidebar*/
</style>

<script setup>
import { ref, watch, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Tooltip from '@/components/base/Tooltip.vue'

const props = defineProps({
  isExpanded: Boolean,
})

const submenuTop = ref(0)
const submenuLeft = computed(() => {
  return props.isExpanded ? 235 : 60 // Khớp với chiều rộng thực tế của Sidebar
})

const handleMouseEnter = (event, item) => {
  if (item.hasDropdown) {
    const rect = event.currentTarget.getBoundingClientRect()
    submenuTop.value = rect.top
  }
}

const menuItems = ref([
  {
    name: 'Tổng quan',
    route: 'overview',
    iconSvg:
      '<g><path d="M7.5,17.5v-5a1.667,1.667,0,0,1,1.667-1.667h1.667A1.667,1.667,0,0,1,12.5,12.5v5M4.167,10H2.5L10,2.5,17.5,10H15.833v5.833A1.667,1.667,0,0,1,14.167,17.5H5.833a1.667,1.667,0,0,1-1.667-1.667Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
  },
  {
    name: 'Thành phần lương',
    route: 'salary-composition',
    iconSvg:
      '<g><path d="M2.5,10,5,12.5,7.5,10,5,7.5Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/><path d="M12.5,10,15,12.5,17.5,10,15,7.5Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/><path d="M7.5,5,10,7.5,12.5,5,10,2.5Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/><path d="M7.5,15,10,17.5,12.5,15,10,12.5Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
  },
  {
    name: 'Mẫu bảng lương',
    route: 'salary-template',
    iconSvg:
      '<g><path d="M11.667,10h5m-5,3.333h5m-5,3.333h5M3.333,4.167a.833.833,0,0,1,.833-.833H15.833a.833.833,0,0,1,.833.833V5.833a.833.833,0,0,1-.833.833H4.167a.833.833,0,0,1-.833-.833Zm0,6.667A.833.833,0,0,1,4.167,10H7.5a.833.833,0,0,1,.833.833v5a.833.833,0,0,1-.833.833H4.167a.833.833,0,0,1-.833-.833Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
  },
  {
    name: 'Dữ liệu tính lương',
    route: 'salary-data',
    iconSvg:
      '<g><path d="M3.333,5c0,.663.7,1.3,1.953,1.768A14.013,14.013,0,0,0,10,7.5a14.012,14.012,0,0,0,4.714-.732c1.25-.469,1.953-1.1,1.953-1.768M3.333,5c0-.663.7-1.3,1.953-1.768A14.013,14.013,0,0,1,10,2.5a14.012,14.012,0,0,1,4.714.732c1.25.469,1.953,1.1,1.953,1.768M3.333,5v5M16.667,5v5M3.333,10c0,.663.7,1.3,1.953,1.768A14.014,14.014,0,0,0,10,12.5a14.013,14.013,0,0,0,4.714-.732c1.25-.469,1.953-1.1,1.953-1.768M3.333,10v5c0,.663.7,1.3,1.953,1.768A14.014,14.014,0,0,0,10,17.5a14.013,14.013,0,0,0,4.714-.732c1.25-.469,1.953-1.1,1.953-1.768V10" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
    hasDropdown: true,
    // dropdownItems are not rendered in this component, but kept for logical completeness
    dropdownItems: [
      { name: 'Chấm công', route: 'timekeeping' },
      { name: 'Doanh số', route: 'sales' },
      { name: 'KPI', route: 'kpi' },
      { name: 'Sản phẩm', route: 'products' },
      { name: 'Thu nhập khác', route: 'other-income' },
      { name: 'Khấu trừ khác', route: 'other-deductions' },
    ],
  },
  {
    name: 'Tính lương',
    route: 'payroll-calculation',
    iconSvg:
      '<g><path d="M2.5,15.833A1.667,1.667,0,0,0,4.167,17.5c1.667,0,1.667-3.333,2.5-7.5s.833-7.5,2.5-7.5a1.667,1.667,0,0,1,1.667,1.667M4.167,10h5M12.5,10l5,5m-5,0,5-5" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
    hasDropdown: true,
    dropdownItems: [
      { name: 'Bảng lương', route: 'payroll-sheet' },
      { name: 'Tạm ứng', route: 'advance-payment' },
      { name: 'Tổng hợp lương', route: 'payroll-summary' },
      { name: 'Phân bổ lương', route: 'salary-allocation' },
      { name: 'Ngân sách lương', route: 'salary-budget' },
      { name: 'Bảng thuế', route: 'tax-sheet' },
      { name: 'Quyết toán thuế', route: 'tax-finalization' },
    ],
  },
  {
    name: 'Chi trả',
    route: 'payment',
    iconSvg:
      '<g><path d="M7.917,2.5h4.167a1.25,1.25,0,0,1,1.25,1.25,2.917,2.917,0,0,1-2.917,2.917H9.583A2.917,2.917,0,0,1,6.667,3.75,1.25,1.25,0,0,1,7.917,2.5Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/><path d="M3.333,14.167v-.833a6.667,6.667,0,0,1,13.333,0v.833A3.333,3.333,0,0,1,13.333,17.5H6.667a3.333,3.333,0,0,1-3.333-3.333Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
    hasDropdown: true,
    dropdownItems: [
      { name: 'Bảng chi trả', route: 'payment-sheet' },
      { name: 'Tổng hợp chi trả', route: 'payment-summary' },
    ],
  },
  {
    name: 'Báo cáo',
    route: 'reports',
    iconSvg:
      '<g><path d="M8.333,2.667a7.5,7.5,0,1,0,9,9,.833.833,0,0,0-.833-.833H10.833A1.667,1.667,0,0,1,9.167,9.167V3.333a.75.75,0,0,0-.833-.667Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/><path d="M12.5,2.917A7.5,7.5,0,0,1,17.083,7.5h-3.75a.833.833,0,0,1-.833-.833Z" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/></g>',
  },
])

const emit = defineEmits(['toggle'])
const router = useRouter()
const route = useRoute()

onMounted(() => {
  const savedState = localStorage.getItem('sidebarExpanded')
  if (savedState !== null) {
    const isExpanded = savedState === 'true'
    if (isExpanded !== props.isExpanded) {
      emit('toggle')
    }
  }
})

watch(
  () => props.isExpanded,
  (newValue) => {
    localStorage.setItem('sidebarExpanded', String(newValue))
  },
)
</script>
