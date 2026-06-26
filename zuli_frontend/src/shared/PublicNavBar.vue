<template>
  <header class="w-full bg-primary text-white">
    <div class="relative flex w-full items-center px-4 py-2 sm:px-6">
      <img src="../assets/logoZuli.svg" alt="Logo" class="h-14 w-auto shrink-0 sm:h-20" />

      <nav class="absolute left-1/2 -translate-x-1/2">
        <ul class="flex items-center divide-x divide-white/40 p-0 m-0 list-none">
          <li class="px-4 first:pl-0">
            <router-link
              to="/"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Reservar
            </router-link>
          </li>
          <li class="px-4">
            <AppButton
              variant="ghost"
              @click="openMaintenance('Check-in')"
              class="!inline-flex !rounded-md !px-3 !py-2 !font-semibold !text-white hover:!bg-black/15 transition"
            >
              Check-in
            </AppButton>
          </li>
          <li class="px-4">
            <router-link
              to="/Mi viaje"
              exact-active-class="nav-active"
              class="inline-flex rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Mi viaje
            </router-link>
          </li>
          <li class="px-4">
            <AppButton
              variant="ghost"
              @click="openMaintenance('Ayuda')"
              class="!inline-flex !rounded-md !px-3 !py-2 !font-semibold !text-white hover:!bg-black/15 transition"
            >
              Ayuda
            </AppButton>
          </li>
        </ul>
      </nav>

      <router-link
        to="/admin"
        class="ml-auto inline-flex items-center gap-2 rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
      >
        <img src="../assets/person.svg" alt="Admin Icon" class="h-6 w-6 shrink-0" />
        <span>Panel administrativo</span>
      </router-link>
    </div>
  </header>

  <ErrorModal 
    v-model="showMaintenanceModal" 
    title="En Mantenimiento" 
    :message="`La sección de ${selectedFeature} se encuentra actualmente en mantenimiento. Por favor, intente más tarde.`" 
    buttonText="Entendido"
  />
</template>

<script setup>
import { ref } from 'vue';
import ErrorModal from './ErrorModal.vue';
import AppButton from './AppButton.vue';

const showMaintenanceModal = ref(false);
const selectedFeature = ref('');

const openMaintenance = (feature) => {
  selectedFeature.value = feature;
  showMaintenanceModal.value = true;
};
</script>

<style scoped>
.nav-active {
  background-color: rgba(0, 0, 0, 0.25);
}
</style>
