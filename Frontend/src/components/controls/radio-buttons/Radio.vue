<template>
  <div
    class="m-radio"
    :class="{
      'm-radio--disabled': disabled,
      'm-radio--active': modelValue === value,
    }"
    @click="handleClick"
  >
    <div class="m-radio-input-wrapper">
      <input
        type="radio"
        :name="name"
        :value="value"
        :checked="modelValue === value"
        :disabled="disabled"
        class="m-radio-input"
        @click.stop
        @change="handleChange"
      />
      <!-- Custom SVG icons -->
      <svg
        v-if="modelValue === value"
        width="20"
        height="20"
        viewBox="80 20 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        class="m-radio-icon"
      >
        <g id="Icon Placeholder_25">
          <g id="Radio button_2">
            <g id="Container_4">
              <mask id="path-39-inside-3_1617_1380" fill="#ffffff">
                <path
                  d="m 82,30 c 0,-4.4183 3.5817,-8 8,-8 4.4183,0 8,3.5817 8,8 0,4.4183 -3.5817,8 -8,8 -4.4183,0 -8,-3.5817 -8,-8 z"
                  id="path8"
                />
              </mask>
              <path
                d="m 82,30 c 0,-4.4183 3.5817,-8 8,-8 4.4183,0 8,3.5817 8,8 0,4.4183 -3.5817,8 -8,8 -4.4183,0 -8,-3.5817 -8,-8 z"
                fill="#ffffff"
                id="path9"
              />
              <path
                d="m 82,30 h 1.5 z m 16,0 h 1.5 z m 0,0 h 1.5 z m -16,0 h 1.5 z m 8,-8 m 8,8 m -8,8 m -8,-8 m 8,8 v -1.5 c -3.5899,0 -6.5,-2.9101 -6.5,-6.5 H 82 80.5 c 0,5.2467 4.2533,9.5 9.5,9.5 z m 8,-8 h -1.5 c 0,3.5899 -2.9101,6.5 -6.5,6.5 v 1.5 1.5 c 5.2467,0 9.5,-4.2533 9.5,-9.5 z m -8,-8 v 1.5 c 3.5899,0 6.5,2.9101 6.5,6.5 h 1.5 1.5 c 0,-5.2467 -4.2533,-9.5 -9.5,-9.5 z m 0,0 v -1.5 c -5.2467,0 -9.5,4.2533 -9.5,9.5 h 1.5 1.5 c 0,-3.5899 2.9101,-6.5 6.5,-6.5 z"
                fill="var(--color-primary)"
                mask="url(#path-39-inside-3_1617_1380)"
                id="path10"
              />
              <circle id="Selection" cx="90" cy="30" r="4" fill="#0e9a62" />
            </g>
          </g>
        </g>
      </svg>
      <svg
        v-else
        width="20"
        height="20"
        viewBox="60 20 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        class="m-radio-icon"
      >
        <g id="Icon Placeholder_24">
          <g id="Radio button">
            <g id="Container_3">
              <mask id="path-36-inside-2_1617_1380" fill="#ffffff">
                <path
                  d="m 62,30 c 0,-4.4183 3.5817,-8 8,-8 4.4183,0 8,3.5817 8,8 0,4.4183 -3.5817,8 -8,8 -4.4183,0 -8,-3.5817 -8,-8 z"
                  id="path5"
                />
              </mask>
              <path
                d="m 62,30 c 0,-4.4183 3.5817,-8 8,-8 4.4183,0 8,3.5817 8,8 0,4.4183 -3.5817,8 -8,8 -4.4183,0 -8,-3.5817 -8,-8 z"
                fill="#ffffff"
                id="path6"
              />
              <path
                d="m 62,30 h 1.5 z m 16,0 h 1.5 z m 0,0 h 1.5 z m -16,0 h 1.5 z m 8,-8 m 8,8 m -8,8 m -8,-8 m 8,8 v -1.5 c -3.5899,0 -6.5,-2.9101 -6.5,-6.5 H 62 60.5 c 0,5.2467 4.2533,9.5 9.5,9.5 z m 8,-8 h -1.5 c 0,3.5899 -2.9101,6.5 -6.5,6.5 v 1.5 1.5 c 5.2467,0 9.5,-4.2533 9.5,-9.5 z m -8,-8 v 1.5 c 3.5899,0 6.5,2.9101 6.5,6.5 h 1.5 1.5 c 0,-5.2467 -4.2533,-9.5 -9.5,-9.5 z m 0,0 v -1.5 c -5.2467,0 -9.5,4.2533 -9.5,9.5 h 1.5 1.5 c 0,-3.5899 2.9101,-6.5 6.5,-6.5 z"
                fill="#d5d7da"
                mask="url(#path-36-inside-2_1617_1380)"
                id="path7"
              />
            </g>
          </g>
        </g>
      </svg>
    </div>
    <label v-if="label" class="m-radio-label">{{ label }}</label>
  </div>
</template>

<script setup>
const props = defineProps({
  modelValue: [String, Number, Boolean],
  value: [String, Number, Boolean],
  label: String,
  name: String,
  disabled: Boolean,
})

const emit = defineEmits(['update:modelValue', 'change'])

const handleClick = () => {
  if (props.disabled) return
  emit('update:modelValue', props.value)
  emit('change', props.value)
}

const handleChange = () => {
  emit('update:modelValue', props.value)
  emit('change', props.value)
}
</script>

<style scoped>
.m-radio {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  user-select: none;
}
.m-radio:not(.m-radio--disabled):hover #path7 {
  fill: var(--color-primary);
}
.m-radio--disabled {
  cursor: not-allowed;
  opacity: 0.6;
}
.m-radio-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
}
.m-radio-input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  width: 100%;
  height: 100%;
  margin: 0;
  z-index: 1;
}
.m-radio-input:disabled {
  cursor: not-allowed;
}
.m-radio-icon {
  flex-shrink: 0;
}
.m-radio-label {
  font-size: 13px;
  color: var(--color-text-main, #212121);
  cursor: inherit;
}
</style>
