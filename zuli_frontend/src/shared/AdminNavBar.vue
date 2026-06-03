<template>
  <header class="w-full bg-primary text-white">
    <div class="relative flex w-full items-center px-4 py-2 sm:px-6">
      <router-link to="/admin/"><img src="../assets/logoZuli.svg" alt="Logo" class="h-14 w-auto shrink-0 sm:h-20" /></router-link>

      <nav class="absolute left-1/2 -translate-x-1/2">
        <ul class="flex items-center divide-x divide-white/40 p-0 m-0 list-none">
          <li class="px-4 first:pl-0">
            <router-link
              to="/admin/aircrafts"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Aeronaves
            </router-link>
          </li>
          <li class="px-4">
            <router-link
              to="/admin/airports"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Aeropuertos
            </router-link>
          </li>
          <li class="px-4">
            <router-link
              to="/admin/routes"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Rutas
            </router-link>
          </li>
          <li class="px-4">
            <router-link
              to="/admin/users"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Usuarios
            </router-link>
          </li>
          <li class="px-4">
            <AppButton
              variant="ghost"
              @click="showMaintenanceModal = true"
              class="!inline-flex !rounded-md !px-3 !py-2 !font-semibold !text-white hover:!bg-black/15 transition"
            >
              Reportes
            </AppButton>
          </li>
        </ul>
      </nav>

      <router-link
        to="/admin/profile-settings"
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
    message="El módulo de Reportes se encuentra actualmente en mantenimiento. Por favor, intente más tarde." 
    buttonText="Entendido"
  />
</template>

<script setup>
import { ref } from 'vue';
import ErrorModal from './ErrorModal.vue';
import AppButton from './AppButton.vue';

const showMaintenanceModal = ref(false);
</script>

<style>
.top-nav-bar {
    background-color: #711717;
    width: 100%;
}

.content {
  width: 100%;
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

.top-content {
    width: 100%;
    align-items: center;
    gap: 20px;
    font-weight: 600;
    color: white;
    font-size: 26px;
}

.logo {
  width: auto;
  height: clamp(48px, 8vw, 100px);
  max-width: 100%;
}

.admin-link {
  margin-left: auto;
  margin-right: 20px;
  color: white;
  text-decoration: none;
  font-weight: 600;
  display: flex;
  flex-direction: column;
  align-items: center;
}

nav ul {
  display: flex; 
  align-items: center;
  list-style: none;
  margin: 0;
  padding: 0;
}

nav ul li {
  padding: 0 18px;
}

nav ul li + li {
  border-left: 1px solid rgba(255, 255, 255, 0.5);
}

.nav-links {
  color: white;
  text-decoration: none;
  font-weight: 600;
}

.nav-links a {
  color: white;
  text-decoration: none;
  font-weight: 600;
  padding: 10px 14px;
  display: inline-block;
}

.nav-links a.router-link-exact-active {
  background-color: rgba(0, 0, 0, 0.25);
  border-radius: 6px;
}

.nav-active {
  background-color: rgba(0, 0, 0, 0.25);
}

</style>