<template>
  <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
    <main
      class="flex flex-1 items-start justify-center bg-[var(--color-secondary)] px-6 pb-36 pt-[140px]"
    >
      <div class="w-full max-w-[735px] overflow-hidden rounded-md bg-white shadow-sm">
        <div class="bg-[var(--color-primary)] px-6 py-6 text-center">
          <h1 class="text-[28px] font-bold tracking-wide text-white">
            Confirmar cancelación de reserva
          </h1>
        </div>

        <div class="bg-white px-16 py-20">
          <div
            v-if="status === 'loading'"
            class="flex flex-col items-center gap-6 text-center"
          >
            <div
              class="h-12 w-12 animate-spin rounded-full border-4 border-[var(--color-primary)] border-t-transparent"
            />

            <p class="text-[16px] text-[var(--color-content)]">
              Procesando la cancelación…
            </p>
          </div>

          <div
            v-else-if="status === 'success'"
            class="flex flex-col items-center gap-6 text-center"
          >
            <div
              class="flex h-16 w-16 items-center justify-center rounded-full bg-green-100"
            >
              <svg
                class="h-8 w-8 text-green-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M5 13l4 4L19 7"
                />
              </svg>
            </div>

            <h2 class="text-[22px] font-bold text-[var(--color-content)]">
              Reserva cancelada
            </h2>

            <p class="text-[16px] text-gray-600">
              {{ resultMessage }}
            </p>
          </div>

          <div
            v-else-if="status === 'invalid'"
            class="flex flex-col items-center gap-6 text-center"
          >
            <div
              class="flex h-16 w-16 items-center justify-center rounded-full bg-red-100"
            >
              <svg
                class="h-8 w-8 text-red-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </div>

            <h2 class="text-[22px] font-bold text-[var(--color-content)]">
              No se puede procesar
            </h2>

            <p class="text-[16px] text-gray-600">
              {{ resultMessage }}
            </p>
          </div>

          <div
            v-else
            class="flex flex-col items-center gap-6 text-center"
          >
            <div
              class="flex h-16 w-16 items-center justify-center rounded-full bg-red-100"
            >
              <svg
                class="h-8 w-8 text-red-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                />
              </svg>
            </div>

            <div class="space-y-2">
              <h2 class="text-[22px] font-bold text-[var(--color-content)]">
                ¿Deseas cancelar tu reserva?
              </h2>

              <p class="text-[16px] text-gray-600">
                Al confirmar, tu reserva quedará cancelada y se anularán todos los pases de abordar asociados.
              </p>
            </div>

            <div class="w-full rounded-lg border border-red-200 bg-red-50 px-6 py-4">
              <p class="text-sm font-semibold text-red-700">
                 Esta acción no es reversible.
              </p>
            </div>

            <div class="mt-4">
              <AppButton
                variant="danger"
                size="lg"
                :loading="isLoading"
                @click="handleConfirm"
              >
                Confirmar cancelación
              </AppButton>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import AppButton from '../../../shared/AppButton.vue';
import { confirmCancellation } from '../service/reservationCancellationService';

const route = useRoute();

const status = ref('idle');
const isLoading = ref(false);
const resultMessage = ref('');
const token = ref('');

onMounted(() => {
  token.value = route.query.token || '';

  if (!token.value) {
    status.value = 'invalid';
    resultMessage.value =
      'La cancelación no puede procesarse. El enlace es inválido, expiró o ya fue utilizado.';
  }
});

const handleConfirm = async () => {
  if (!token.value) {
    status.value = 'invalid';
    resultMessage.value =
      'La cancelación no puede procesarse. El enlace es inválido, expiró o ya fue utilizado.';
    return;
  }

  isLoading.value = true;
  status.value = 'loading';

  try {
    await confirmCancellation(token.value);

    status.value = 'success';
    resultMessage.value = 'La reserva fue cancelada correctamente.';
  } catch (error) {
    status.value = 'invalid';

    resultMessage.value =
      error?.response?.data?.message ||
      error?.response?.data?.errors?.token?.[0] ||
      error?.response?.data?.errors?.global ||
      error?.message ||
      'La cancelación no puede procesarse. El enlace es inválido, expiró o ya fue utilizado.';
  } finally {
    isLoading.value = false;
  }
};
</script>