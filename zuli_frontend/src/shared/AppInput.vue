<script setup>
defineProps({
  label: { type: String, default: '' },
  modelValue: { type: [String, Number], default: '' },
  type: { type: String, default: 'text' },
  placeholder: { type: String, default: '' },
  error: { type: String, default: '' },
  disabled: { type: Boolean, default: false },
  required: { type: Boolean, default: false },
})

const emit = defineEmits(['update:modelValue'])

function onInput(e) {
  emit('update:modelValue', e.target.value)
}
</script>

<template>
  <div class="form-field group">
    <input
      :id="label"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder || ' '"
      :disabled="disabled"
      :required="required"
      class="form-input peer"
      :class="{ 'border-error': error }"
      @input="onInput"
    />
    <label
      v-if="label"
      :for="label"
      class="form-label"
      :class="{ '!text-error': error }"
    >
      {{ label }}
      <span v-if="required" class="text-error">*</span>
    </label>
    <p v-if="error" class="mt-1 text-sm text-error">{{ error }}</p>
  </div>
</template>

<style scoped>
@reference "../../style.css";
.form-field { @apply relative; }
.form-input {
  @apply block w-full appearance-none border-0 border-b-2 border-gray-300 bg-transparent px-0 py-2.5 text-content focus:border-primary focus:outline-none focus:ring-0 disabled:cursor-not-allowed disabled:opacity-50;
}
.form-label {
  @apply absolute top-3 -z-10 origin-[0] -translate-y-6 scale-75 transform text-sm text-gray-500 duration-300 peer-placeholder-shown:translate-y-0 peer-placeholder-shown:scale-100 peer-focus:-translate-y-6 peer-focus:scale-75 peer-focus:text-primary;
}
</style>
