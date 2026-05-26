<script setup>
defineProps({
  variant: { type: String, default: 'primary' },
  size: { type: String, default: 'md' },
  loading: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false },
  type: { type: String, default: 'button' },
})

const emit = defineEmits(['click'])

function handleClick(e) {
  emit('click', e)
}
</script>

<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    class="inline-flex items-center justify-center gap-2 rounded-xl font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-primary/50 disabled:cursor-not-allowed disabled:opacity-50"
    :class="[
      size === 'sm' && 'px-4 py-2 text-sm',
      size === 'md' && 'px-6 py-3 text-base',
      size === 'lg' && 'px-8 py-4 text-lg',
      variant === 'primary' && 'bg-primary text-white hover:bg-select',
      variant === 'secondary' && 'bg-secondary text-content hover:bg-secondary/70',
      variant === 'outline' && 'border-2 border-primary text-primary hover:bg-primary hover:text-white',
      variant === 'ghost' && 'text-primary hover:bg-primary/10',
      variant === 'danger' && 'bg-error text-white hover:opacity-90',
      variant === 'link' && 'text-primary hover:text-gold hover:underline p-0',
    ]"
    @click="handleClick"
  >
    <span v-if="loading" class="inline-block h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
    <slot />
  </button>
</template>
