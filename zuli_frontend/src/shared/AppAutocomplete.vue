<script setup>
import { ref } from 'vue'
import AppInput from './AppInput.vue'

defineProps({
  modelValue: { type: [String, Number], default: '' },
  label: { type: String, default: '' },
  error: { type: String, default: '' },
  placeholder: { type: String, default: '' },
  disabled: { type: Boolean, default: false },
  required: { type: Boolean, default: false },
  maxlength: { type: [Number, String], default: null },
  suggestions: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false },
})

const emit = defineEmits(['update:modelValue', 'select'])

const isOpen = ref(false)

function onInput(value) {
  emit('update:modelValue', value)
  isOpen.value = true
}

function onSelect(suggestion) {
  emit('select', suggestion)
  isOpen.value = false
}

function onFocus() {
  isOpen.value = true
}

function onBlur() {
  setTimeout(() => {
    isOpen.value = false
  }, 150)
}
</script>

<template>
  <div class="autocomplete-wrapper">
    <AppInput
      :model-value="modelValue"
      :label="label"
      :error="error"
      :placeholder="placeholder"
      :disabled="disabled"
      :required="required"
      :maxlength="maxlength"
      @update:model-value="onInput"
      @focus="onFocus"
      @blur="onBlur"
    />
    <div v-if="isOpen && suggestions.length" class="suggestion-list">
      <button
        v-for="(suggestion, index) in suggestions"
        :key="index"
        type="button"
        class="suggestion-item"
        @mousedown.prevent="onSelect(suggestion)"
      >
        <slot name="suggestion" :suggestion="suggestion">
          <span>{{ suggestion }}</span>
        </slot>
      </button>
    </div>
  </div>
</template>

<style scoped>
@reference "../style.css";

.autocomplete-wrapper {
  @apply relative;
}

.suggestion-list {
  @apply absolute z-20 mt-1 w-full rounded-md border border-gray-200 bg-white shadow-lg;
}

.suggestion-item {
  @apply flex w-full items-start gap-2 px-3 py-2 text-left text-sm text-gray-900 hover:bg-gray-100;
}
</style>
