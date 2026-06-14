<template>
  <svg
    :width="size"
    :height="size"
    viewBox="0 0 16 16"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <g id="Icon Placeholder_16">
      <path
        id="Vector_15"
        d="M13 3L3 13M3 3L13 13"
        stroke="currentColor"
        stroke-width="1.5"
        stroke-linecap="round"
        stroke-linejoin="round"
      />
    </g>
  </svg>
</template>

<script setup>
defineProps({
  size: {
    type: String,
    default: '12',
  },
})
</script>
