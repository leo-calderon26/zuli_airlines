<template>
  <section class="rounded-xl border border-[#dcc0be]/20 bg-white p-6 shadow-sm">
    <div class="mb-4 flex items-center gap-2">
      <span class="h-4 w-4 rounded-sm bg-primary"></span>

      <h2 class="text-base font-semibold text-content">
        {{ title }}
      </h2>
    </div>

    <div class="space-y-4">
      <slot />
    </div>
  </section>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true,
  },
})
</script>