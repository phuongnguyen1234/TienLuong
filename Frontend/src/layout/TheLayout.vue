<template>
  <TheHeader />
  <TheSidebar :isExpanded="isSidebarExpanded" @toggle="toggleSidebar" />
  <div class="main-content" :class="{ content_expanded: isSidebarExpanded }">
    <router-view></router-view>
  </div>
  <Toast />
</template>

<script setup>
import { ref } from 'vue'
import TheHeader from './TheHeader.vue'
import TheSidebar from './TheSidebar.vue'
import Toast from '@/components/Toast.vue'

const isSidebarExpanded = ref(false)

const toggleSidebar = () => {
  isSidebarExpanded.value = !isSidebarExpanded.value
}
</script>

<style scoped>
.main-content {
  position: absolute;
  top: 48px; /* Khớp với chiều cao Header */
  left: var(--sidebar-width-collapsed); /* Mặc định khi sidebar thu nhỏ */
  right: 0;
  bottom: 0;
  transition: left 0.3s ease;
  overflow: hidden;
  background-color: #f2f2f2; /* Màu nền từ Design.md */
  display: flex;
  flex-direction: column;
}

/* Khi sidebar mở rộng, đẩy vùng nội dung sang phải */
.content_expanded {
  left: var(--sidebar-width-expanded);
}
</style>
