<script setup>
import { watch } from 'vue'
import AppButton from './AppButton.vue'

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  title: { type: String, default: '¡Operación exitosa!' },
  message: { type: String, default: 'La acción se completó correctamente.' },
  buttonText: { type: String, default: 'Aceptar' },
})

const emit = defineEmits(['update:modelValue', 'close'])

function close() {
  emit('update:modelValue', false)
  emit('close')
}

function onBackdropClick(e) {
  if (e.target === e.currentTarget) close()
}

function onKeydown(e) {
  if (e.key === 'Escape') close()
}

watch(() => props.modelValue, (val) => {
  if (val) document.addEventListener('keydown', onKeydown)
  else document.removeEventListener('keydown', onKeydown)
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="modelValue"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm"
        @click="onBackdropClick"
      >
        <div class="w-full max-w-md rounded-2xl bg-white p-8 shadow-2xl">
          <div class="flex flex-col items-center text-center">
            <div class="mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-green-100">
              <svg class="h-8 w-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <h2 class="mb-2 text-xl font-bold text-content">{{ title }}</h2>
            <p class="mb-6 text-gray-600">{{ message }}</p>
            <AppButton variant="primary" @click="close">
              {{ buttonText }}
            </AppButton>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active, .modal-leave-active {
  transition: opacity 0.25s ease;
}
.modal-enter-active > div, .modal-leave-active > div {
  transition: transform 0.25s ease;
}
.modal-enter-from, .modal-leave-to {
  opacity: 0;
}
.modal-enter-from > div {
  transform: scale(0.9);
}
.modal-leave-to > div {
  transform: scale(0.9);
}
</style>
