<script setup>
import { watch } from 'vue'
import AppButton from './AppButton.vue'

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  title: { type: String, default: 'Error' },
  message: { type: String, default: 'Ocurrió un error al procesar la solicitud.' },
  errors: { type: Object, default: null },
  buttonText: { type: String, default: 'Cerrar' },
})

const emit = defineEmits(['update:modelValue', 'confirm', 'close'])

function close() {
  emit('update:modelValue', false)
  emit('close')
}

function proceed() {
  emit('update:modelValue', false)
  emit('confirm')
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
            <div class="mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-yellow-100">
              <svg class="h-8 w-8 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v4m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" />
              </svg>
            </div>
            <h2 class="mb-2 text-xl font-bold text-content">{{ title }}</h2>
            <p class="mb-4 text-gray-600 whitespace-pre-line">{{ message }}</p>
            <p v-if="$slots.default" class="mb-4 text-sm text-error"><slot /></p>

            <div
              v-if="errors && Object.keys(errors).length"
              class="mb-4 w-full rounded-lg bg-red-50 p-4 text-left"
            >
              <ul class="list-inside list-disc space-y-1 text-sm text-error">
                <li v-for="(err, key) in errors" :key="key">
                  <span v-if="key !== 'global'" class="font-medium">{{ key }}:</span>
                  {{ Array.isArray(err) ? err.join(', ') : err }}
                </li>
              </ul>
            </div>
            <div class="flex gap-2">
              <AppButton
                class="rounded-md bg-primary px-2 py-2 text-sm font-semibold text-white hover:bg-select"
                @click="proceed">
                {{ buttonText }}
              </AppButton>

              <AppButton
                class="rounded-md bg-primary px-2 py-2 text-sm font-semibold text-white hover:bg-select"
                @click="close">
                Cancelar
              </AppButton>
            </div>
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
