<script setup>
defineProps({
  label: { type: String, default: '' },
  modelValue: { type: [String, Number], default: '' },
  type: { type: String, default: 'text' },
  placeholder: { type: String, default: '' },
  error: { type: String, default: '' },
  disabled: { type: Boolean, default: false },
  required: { type: Boolean, default: false },
  maxlength: { type: [Number, String], default: null },
  min: { type: [Number, String], default: null },
  step: { type: [Number, String], default: null },
})

const emit = defineEmits(['update:modelValue', 'focus', 'blur'])

function onInput(e) {
  emit('update:modelValue', e.target.value)
}

function onFocus(e) {
  emit('focus', e)
}

function onBlur(e) {
  emit('blur', e)
}
</script>

<template>
  <div class="form-field">
    <label
      v-if="label"
      :for="label"
      class="form-label"
      :class="{ '!text-error': error }"
    >
      {{ label }}
      <span v-if="required" class="text-error">*</span>
    </label>
    <input
      :id="label"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder || undefined"
      :disabled="disabled"
      :required="required"
      :maxlength="maxlength"
      :min="min"
      :step="step"
      class="form-input"
      :class="{ 'border-error text-error': error }"
      @input="onInput"
      @focus="onFocus"
      @blur="onBlur"
    />
    <p v-if="error" class="mt-1 text-sm text-error">{{ error }}</p>
  </div>
</template>

<style scoped>
@reference "../style.css";
.form-field {
  @apply flex flex-col;
}
.form-input {
  @apply block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 placeholder:text-font/40 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:cursor-not-allowed disabled:opacity-50;
}
.form-label {
  @apply mb-2 block text-sm font-medium text-content;
}
</style>

