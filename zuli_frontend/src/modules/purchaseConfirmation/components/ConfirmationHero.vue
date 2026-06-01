<template>
  <section class="rounded-xl bg-white p-6 shadow-md">
    <div class="flex flex-col gap-6 lg:flex-row lg:items-center">
      <div class="flex h-24 w-24 shrink-0 items-center justify-center rounded-full bg-primary/10">
        <svg
          class="h-12 w-12 text-primary"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2.2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path d="M20 6 9 17l-5-5" />
        </svg>
      </div>

      <div class="flex-1">
        <h1 class="text-3xl font-semibold text-primary">
          Reserva completada.
        </h1>

        <p class="mt-2 text-base text-[#554241]">
          {{ message }}
        </p>

        <div class="mt-4 inline-flex items-center gap-3 rounded-lg border border-[#dcc0be]/30 bg-[#F4F3F3] px-4 py-2">
          <span class="text-sm uppercase text-[#554241]">
            Código de reserva
          </span>

          <span class="font-semibold tracking-widest text-primary">
            {{ reservationCode }}
          </span>
        </div>
      </div>

      <div class="hidden h-24 w-px bg-[#dcc0be]/30 lg:block"></div>

      <div class="min-w-60 space-y-3">
        <StatusItem
          label="Factura enviada"
          :active="invoiceEmailSent"
        />

        <StatusItem
          label="Confirmación e itinerario enviados"
          :active="confirmationEmailSent"
        />
      </div>
    </div>
  </section>
</template>

<script setup>
defineProps({
  message: {
    type: String,
    default: 'Los detalles han sido enviados a su correo electrónico.',
  },
  reservationCode: {
    type: String,
    required: true,
  },
  invoiceEmailSent: {
    type: Boolean,
    default: false,
  },
  confirmationEmailSent: {
    type: Boolean,
    default: false,
  },
})
</script>

<script>
export default {
  components: {
    StatusItem: {
      props: {
        label: {
          type: String,
          required: true,
        },
        active: {
          type: Boolean,
          required: true,
        },
      },
      template: `
        <div class="flex items-center gap-3">
          <span
            class="flex h-5 w-5 items-center justify-center rounded-full text-xs text-white"
            :class="active ? 'bg-gold' : 'bg-gray-300'"
          >
            {{ active ? '✓' : '!' }}
          </span>

          <span class="text-content">
            {{ label }}
          </span>
        </div>
      `,
    },
  },
}
</script>