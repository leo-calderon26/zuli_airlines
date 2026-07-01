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
      />

    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { onBeforeRouteLeave } from 'vue-router';
import { useReservationSearch } from '../composable/useReservationSearch';
import ReservationSearchForm from '../components/ReservationSearchForm.vue';
import ReservationResult from '../components/ReservationResult.vue';

const { search, clear, store } = useReservationSearch();

const hasResults = computed(() => store.reservationData !== null);

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
</script>
