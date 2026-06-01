<template>
  <InfoCard title="Datos del comprador">
    <InfoRow
      label="Nombre completo"
      :value="buyerName"
    />

    <InfoRow
      label="Correo electrónico"
      :value="buyerEmail"
    />

    <InfoRow
      label="Teléfono"
      :value="buyerPhone"
    />
  </InfoCard>
</template>

<script setup>
import InfoCard from './InfoCard.vue'

defineProps({
  buyerName: {
    type: String,
    default: '-',
  },
  buyerEmail: {
    type: String,
    default: '-',
  },
  buyerPhone: {
    type: String,
    default: '-',
  },
})
</script>

<script>
export default {
  components: {
    InfoRow: {
      props: {
        label: {
          type: String,
          required: true,
        },
        value: {
          type: [String, Number],
          default: '-',
        },
      },
      template: `
        <div>
          <p class="text-[#554241]">
            {{ label }}
          </p>

          <p class="break-words font-semibold text-content">
            {{ value || '-' }}
          </p>
        </div>
      `,
    },
  },
}
</script>