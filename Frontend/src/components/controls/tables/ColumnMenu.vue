<template>
  <Teleport to="body">
    <div v-if="visible" class="column-menu" :style="style" ref="menuRef">
      <ul class="menu-list">
        <li
          v-for="item in menuOptions"
          :key="item.id"
          class="menu-item"
          @click="handleAction(item)"
        >
          <div class="item-content-left">
            <span class="item-icon" v-html="item.icon"></span>
            <span class="item-label">{{ item.label }}</span>
          </div>
          <svg
            v-if="item.active"
            class="tick-icon"
            width="16"
            height="16"
            viewBox="0 0 16 16"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M13.3333 4L6 11.3333L2.66667 8"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </li>
      </ul>
    </div>
  </Teleport>
</template>

<script setup>
import { computed, ref, onMounted, onBeforeUnmount } from 'vue'

const props = defineProps({
  visible: Boolean,
  column: Object,
  sortOrder: String,
  isPinned: Boolean,
  top: String,
  left: String,
})

const emit = defineEmits(['update:visible', 'sort', 'pin'])
const menuRef = ref(null)

const menuOptions = computed(() => [
  {
    id: 'none',
    label: 'Không sắp xếp',
    icon: '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M13.3615 10.6967C13.9296 9.56931 14.1275 8.29146 13.9272 7.04503C13.7269 5.7986 13.1385 4.6471 12.2458 3.75444C11.3532 2.86177 10.2017 2.27341 8.95525 2.07309C7.70882 1.87278 6.43096 2.07071 5.30355 2.63873M3.75821 3.75673C3.1935 4.3121 2.74434 4.97378 2.43664 5.70361C2.12893 6.43343 1.96877 7.21696 1.96539 8.009C1.96201 8.80103 2.11548 9.5859 2.41694 10.3183C2.71841 11.0507 3.16191 11.7162 3.72186 12.2764C4.28181 12.8366 4.94712 13.2803 5.67943 13.5821C6.41174 13.8838 7.19654 14.0376 7.98858 14.0346C8.78061 14.0315 9.5642 13.8716 10.2942 13.5642C11.0241 13.2568 11.686 12.8079 12.2415 12.2434M2.0002 2.00006L14.0002 14.0001" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    active: props.sortOrder === null,
  },
  {
    id: 'asc',
    label: 'Tăng dần',
    icon: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 19V5M5 12l7-7 7 7"/></svg>',
    active: props.sortOrder === 'asc',
  },
  {
    id: 'desc',
    label: 'Giảm dần',
    icon: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 5v14M5 12l7 7 7-7"/></svg>',
    active: props.sortOrder === 'desc',
  },
  {
    id: 'pin',
    label: 'Ghim cột',
    icon: '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M5.99984 2.66675V6.66675L4.6665 9.33341V10.6667H11.3332V9.33341L9.99984 6.66675V2.66675M7.99984 10.6667V14.0001M5.33317 2.66675H10.6665" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    active: props.isPinned,
  },
])

const style = computed(() => ({ top: props.top, left: props.left }))

function handleAction(item) {
  if (item.id === 'pin') {
    emit('pin', props.column)
  } else {
    emit('sort', props.column.key, item.id === 'none' ? null : item.id)
  }
  emit('update:visible', false)
}

function handleClickOutside(event) {
  if (menuRef.value && !menuRef.value.contains(event.target)) {
    emit('update:visible', false)
  }
}

onMounted(() => document.addEventListener('mousedown', handleClickOutside))
onBeforeUnmount(() => document.removeEventListener('mousedown', handleClickOutside))
</script>

<style scoped>
.column-menu {
  position: fixed;
  background-color: #ffffff;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 9999;
  padding: 8px 0;
  min-width: 200px;
}
.menu-list {
  list-style: none;
  margin: 0;
  padding: 0;
}
.menu-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  cursor: pointer;
  font-size: 13px;
  color: #212121;
}
.menu-item:hover {
  background-color: #f2f2f2;
}
.item-content-left {
  display: flex;
  align-items: center;
  gap: 10px;
}
.item-icon {
  display: flex;
  align-items: center;
  color: var(--color-text-secondary);
  width: 16px;
  height: 16px;
}
.tick-icon {
  color: var(--color-text-secondary);
}
</style>
