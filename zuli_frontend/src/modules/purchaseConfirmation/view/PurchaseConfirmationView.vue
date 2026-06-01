<template>
  <div class="min-h-screen bg-secondary text-content">
    <PurchaseConfirmationTopBar
      @go-home="goHome"
      @go-destination="goDestination"
      @go-reservation="goReservation"
    />

    <main class="page-shell py-10">
      <section
        v-if="isLoading"
        class="rounded-xl bg-white p-8 shadow-sm"
      >
        <p class="text-lg font-semibold text-primary">
          Cargando confirmación de compra...
        </p>
      </section>

      <section
        v-else-if="pageError"
        class="rounded-[18px] border border-error/30 bg-white p-8 shadow-sm"
      >
        <h1 class="text-2xl font-semibold text-error">
          Error de Carga
        </h1>

        <p class="mt-3 text-content">
          {{ pageError }}
        </p>

        <button
          type="button"
          class="mt-6 inline-flex items-center gap-2 rounded-full bg-primary px-6 py-3 text-white shadow-md transition hover:opacity-90"
          @click="goHome"
        >
          <span>Inicio</span>

          <svg
            class="h-4 w-4"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M3 10.5 12 3l9 7.5" />
            <path d="M5 10v10h14V10" />
            <path d="M9 20v-6h6v6" />
          </svg>
        </button>
      </section>

      <template v-else-if="confirmation">
        <ConfirmationHero
          :message="confirmation.message"
          :reservation-code="confirmation.reservationCode"
          :invoice-email-sent="confirmation.invoiceEmailSent"
          :confirmation-email-sent="confirmation.confirmationEmailSent"
        />

        <section class="mt-6 grid gap-6 lg:grid-cols-[360px_1fr]">
          <div class="space-y-6">
            <BuyerInfoCard
              :buyer-name="confirmation.buyerName"
              :buyer-email="confirmation.buyerEmail"
              :buyer-phone="confirmation.buyerPhone"
            />

            <PaymentSummaryCard
              :payment-method="confirmation.paymentMethod"
              :flight-class="confirmation.flightClass"
              :total-amount="confirmation.totalAmount"
            />
          </div>

          <div class="space-y-6">
            <ItineraryCard :flights="confirmation.flights" />

            <PassengerTable :passengers="confirmation.passengers" />

            <div class="flex justify-end">
              <button
                type="button"
                class="inline-flex items-center gap-2 rounded-full bg-primary px-8 py-4 text-white shadow-lg transition hover:opacity-90"
                @click="goHome"
              >
                <span>Volver al inicio</span>

                <svg
                  class="h-4 w-4"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                >
                  <path d="M3 10.5 12 3l9 7.5" />
                  <path d="M5 10v10h14V10" />
                  <path d="M9 20v-6h6v6" />
                </svg>
              </button>
            </div>
          </div>
        </section>
      </template>
    </main>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import BuyerInfoCard from '../components/BuyerInfoCard.vue'
import ConfirmationHero from '../components/ConfirmationHero.vue'
import ItineraryCard from '../components/ItineraryCard.vue'
import PassengerTable from '../components/PassengerTable.vue'
import PaymentSummaryCard from '../components/PaymentSummaryCard.vue'
import PurchaseConfirmationTopBar from '../components/PurchaseConfirmationTopBar.vue'

import {
  completePurchaseConfirmation,
  getPurchaseConfirmation,
} from '../service/purchaseConfirmationService'

const route = useRoute()
const router = useRouter()

const confirmation = ref(null)
const isLoading = ref(false)
const pageError = ref('')

const reservationId = computed(() => route.params.reservationId)

onMounted(async () => {
  await loadConfirmation()
})

async function loadConfirmation() {
  if (!reservationId.value) {
    pageError.value = 'No se recibió el identificador de la reserva.'
    return
  }

  isLoading.value = true
  pageError.value = ''

  try {
    const shouldCompletePurchase = route.query.complete === 'true'

    confirmation.value = shouldCompletePurchase
      ? await completePurchaseConfirmation(reservationId.value)
      : await getPurchaseConfirmation(reservationId.value)
  } catch (error) {
    pageError.value = getErrorMessage(error)
  } finally {
    isLoading.value = false
  }
}

function getErrorMessage(error) {
  const responseData = error?.response?.data

  if (responseData?.detail) {
    return responseData.detail
  }

  if (responseData?.message) {
    return responseData.message
  }

  if (responseData?.errors) {
    return 'La reserva tiene información incompleta o inválida.'
  }

  return 'No se pudo cargar la confirmación de compra.'
}

function goHome() {
  router.push('/')
}

function goDestination() {
  router.push('/')
}

function goReservation() {
  router.push('/')
}
</script>