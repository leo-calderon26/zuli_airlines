<template>
  <main class="min-h-screen bg-[#FDF4EF] text-[#2f1b18]">
    <PurchaseConfirmationTopBar
      @go-home="goHome"
      @go-destination="goDestination"
      @go-reservation="goReservation"
    />

    <section class="px-6 py-10">
      <div class="mx-auto max-w-6xl">
        <div
          v-if="isLoading"
          class="flex min-h-[520px] items-center justify-center"
        >
          <div class="rounded-3xl bg-white px-10 py-8 text-center shadow-sm">
            <p class="text-lg font-semibold text-[#701919]">
              Cargando confirmación...
            </p>
          </div>
        </div>

        <div
          v-else-if="pageError"
          class="flex min-h-[520px] items-center justify-center"
        >
          <div class="max-w-xl rounded-3xl bg-white px-10 py-8 text-center shadow-sm">
            <div class="mx-auto mb-5 flex h-16 w-16 items-center justify-center rounded-full bg-[#701919]/10">
              <svg
                class="h-8 w-8 text-[#701919]"
                viewBox="0 0 24 24"
                fill="currentColor"
                aria-hidden="true"
              >
                <path d="M11 7h2v7h-2V7Zm0 9h2v2h-2v-2Z" />
                <path d="M12 2 1 21h22L12 2Zm0 4.04L19.53 19H4.47L12 6.04Z" />
              </svg>
            </div>

            <h1 class="mb-3 text-2xl font-bold text-[#701919]">
              No se pudo cargar la confirmación
            </h1>

            <p class="mb-8 text-sm text-gray-600">
              {{ pageError }}
            </p>

            <button
              type="button"
              class="inline-flex items-center gap-2 rounded-full bg-[#701919] px-7 py-3 text-sm font-semibold text-white shadow-sm transition hover:bg-[#5d1515]"
              @click="goHome"
            >
              <svg
                class="h-5 w-5"
                viewBox="0 0 24 24"
                fill="currentColor"
                aria-hidden="true"
              >
                <path d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8h5Z" />
              </svg>

              Inicio
            </button>
          </div>
        </div>

        <div v-else-if="confirmation">
          <ConfirmationHero
            :reservation-code="confirmation.reservationCode"
            :message="confirmation.message"
            :emails-sent="confirmation.emailsSent"
          />

          <div class="mt-8 grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
            <div class="space-y-6">
              <ItineraryCard :flights="confirmation.flights" />
              <PassengerTable :passengers="confirmation.passengers" />
            </div>

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
          </div>
        </div>
      </div>
    </section>
  </main>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import PurchaseConfirmationTopBar from '../components/PurchaseConfirmationTopBar.vue'
import ConfirmationHero from '../components/ConfirmationHero.vue'
import ItineraryCard from '../components/ItineraryCard.vue'
import PassengerTable from '../components/PassengerTable.vue'
import BuyerInfoCard from '../components/BuyerInfoCard.vue'
import PaymentSummaryCard from '../components/PaymentSummaryCard.vue'
import { getPurchaseConfirmation } from '../service/purchaseConfirmationService'

const route = useRoute()
const router = useRouter()

const confirmation = ref(null)
const isLoading = ref(false)
const pageError = ref('')

const reservationCode = computed(() => route.params.reservationCode)

onMounted(async () => {
  await loadConfirmation()
})

async function loadConfirmation() {
  if (!reservationCode.value) {
    pageError.value = 'No se recibió el código de la reserva.'
    return
  }

  isLoading.value = true
  pageError.value = ''

  try {
    confirmation.value = await getPurchaseConfirmation(reservationCode.value)
  } catch (error) {
    pageError.value = getErrorMessage(error)
  } finally {
    isLoading.value = false
  }
}

function getErrorMessage(error) {
  return error?.response?.data?.message
    || error?.response?.data?.detail
    || error?.message
    || 'Ocurrió un error al cargar la confirmación.'
}

function goHome() {
  router.push({ name: 'reserve' })
}

function goDestination() {
  router.push({ name: 'buscarVuelos' })
}

function goReservation() {
  router.push({ name: 'reserve' })
}
</script>