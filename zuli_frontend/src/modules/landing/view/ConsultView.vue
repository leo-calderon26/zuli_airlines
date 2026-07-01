<template>
  <div
    class="relative min-h-[calc(100vh-84px)] bg-cover bg-center bg-no-repeat bg-fixed overflow-x-hidden flex flex-col"
    style="background-image: url('https://images.unsplash.com/photo-1483450388369-9ed95738483c?q=80&w=2070&auto=format&fit=crop');"
  >
    <div class="absolute top-0 left-0 right-0 h-80 bg-gradient-to-b from-primary to-transparent opacity-95 z-0 pointer-events-none"></div>
    <div class="absolute inset-0 bg-black/40 z-0 pointer-events-none"></div>

    <main class="relative z-10 flex-1 flex flex-col pt-16 px-4 pb-20 w-full max-w-6xl mx-auto">

      <ReservationSearchForm
        v-if="!hasResults"
        :is-loading="store.isLoading"
        :error="store.error"
        @search="handleSearch"
      />

      <ReservationResult
        v-else
        :reservation-data="store.reservationData"
        @clear="handleClear"
        @request-cancel="onRequestCancel"
      />

    </main>

    <AlertModal
      v-model="showCancelConfirmModal"
      title="Cancelar reserva"
      :message="cancelConfirmMessage"
      buttonText="Confirmar"
      @confirm="onCancelConfirmed"
    />
  
    <SuccessModal
      v-model="showSuccessModal"
      title="Solicitud enviada"
      :message="successMessage"
    />

    <AlertModal
      v-model="showErrorModal"
      title="Error"
      :message="errorMessage"
      buttonText="Cerrar"
    />

  </div>
</template>

<script setup>
import { onBeforeRouteLeave } from 'vue-router';
import { computed, ref } from 'vue';
import { useReservationSearch } from '../composable/useReservationSearch';
import ReservationSearchForm from '../components/ReservationSearchForm.vue';
import ReservationResult from '../components/ReservationResult.vue';
import AlertModal from '../../../shared/AlertModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import { requestCancellation } from '../service/reservationCancellationService';

const { search, clear, store } = useReservationSearch();

const hasResults = computed(() => store.reservationData !== null);

const showCancelConfirmModal = ref(false);
const showSuccessModal = ref(false);
const showErrorModal = ref(false);

const cancelConfirmMessage = 'Se enviarán instrucciones al correo del comprador registrado para completar la cancelación. La reserva no se cancela en este paso.';
const successMessage = ref('Revisa el correo del comprador para continuar.');
const errorMessage = ref('');


const handleSearch = async ({ reservationCode, lastName }) => {
  await search(reservationCode.toUpperCase().trim(), lastName.trim());
};

const handleClear = () => {
  clear();
};

onBeforeRouteLeave((to) => {
  if (to.name !== 'additionalBaggage') {
    clear();
  }
});

const onRequestCancel = () => {
  showCancelConfirmModal.value = true;
};

const onCancelConfirmed = async () => {
  const reservationCode = store.reservationData?.reservationCode;
  try {
    await requestCancellation(reservationCode);
    successMessage.value ='Se enviaron instrucciones al correo del comprador para completar la cancelación.';
    showSuccessModal.value = true;
  } catch (error) {
    const msg =
      error?.response?.data?.message ||
      error?.response?.data?.errors?.global ||
      error?.message ||
      'No se pudo enviar la solicitud de cancelación. Intente de nuevo.';
    errorMessage.value = msg;
    showErrorModal.value = true;
  }
};
</script>
