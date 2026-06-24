<template>
  <header class="sticky top-0 z-50 w-full bg-primary text-white shadow-md">
    <div class="mx-auto flex h-18 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
      <router-link to="/admin/" class="flex items-center gap-2 transition hover:opacity-90">
        <img src="../assets/logoZuli.svg" alt="Zuli Airlines" class="h-12 w-auto sm:h-14" />
      </router-link>

      <nav class="hidden lg:block">
        <ul class="flex items-center gap-1">
          <li v-for="item in menuItems" :key="item.route">
  <header class="sticky top-0 z-50 w-full bg-primary text-white shadow-md">
    <div class="mx-auto flex h-18 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
      <router-link to="/admin/" class="flex items-center gap-2 transition hover:opacity-90">
        <img src="../assets/logoZuli.svg" alt="Zuli Airlines" class="h-12 w-auto sm:h-14" />
      </router-link>

      <nav class="hidden lg:block">
        <ul class="flex items-center gap-1">
          <li v-for="item in menuItems" :key="item.route">
            <router-link
              :to="item.route"
              :to="item.route"
              exact-active-class="nav-active"
              class="rounded-lg px-4 py-2 text-base font-medium text-white/90 transition hover:bg-black/15 hover:text-white"
            >
              {{ item.label }}
            </router-link>
          </li>
        </ul>
      </nav>

      <div class="hidden items-center gap-2 lg:flex">
        <router-link
          to="/admin/profile-settings"
          exact-active-class="nav-active"
          class="rounded-lg px-3 py-2 text-base font-medium text-white/90 transition hover!bg-black/15 hover:text-white"
        >
          Cerrar Sesión
        </router-link>
      </div>

      <button
        type="button"
        class="inline-flex items-center justify-center rounded-lg p-2 text-white/90 transition hover:bg-black/15 hover:text-white lg:hidden"
        aria-label="Abrir menú"
        :aria-expanded="mobileMenuOpen"
        @click="toggleMobileMenu"
      >
        <Bars3Icon v-if="!mobileMenuOpen" class="h-7 w-7" />
        <XMarkIcon v-else class="h-7 w-7" />
      </button>
      <div class="hidden items-center gap-2 lg:flex">
        <router-link
          to="/admin/profile-settings"
          exact-active-class="nav-active"
          class="rounded-lg px-3 py-2 text-base font-medium text-white/90 transition hover!bg-black/15 hover:text-white"
        >
          Cerrar Sesión
        </router-link>
      </div>

      <button
        type="button"
        class="inline-flex items-center justify-center rounded-lg p-2 text-white/90 transition hover:bg-black/15 hover:text-white lg:hidden"
        aria-label="Abrir menú"
        :aria-expanded="mobileMenuOpen"
        @click="toggleMobileMenu"
      >
        <Bars3Icon v-if="!mobileMenuOpen" class="h-7 w-7" />
        <XMarkIcon v-else class="h-7 w-7" />
      </button>
    </div>

    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="-translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="-translate-y-2 opacity-0"
    >
      <div
        v-if="mobileMenuOpen"
        class="border-t border-white/10 bg-primary lg:hidden"
      >
        <nav class="mx-auto max-w-7xl space-y-1 px-4 py-3 sm:px-6">
          <router-link
            v-for="item in menuItems"
            :key="item.route"
            :to="item.route"
            exact-active-class="nav-active"
            class="block rounded-lg px-3 py-3 text-base font-medium text-white/90 transition hover:bg-white/10 hover:text-white"
            @click="closeMobileMenu"
          >
            {{ item.label }}
          </router-link>

          <div class="my-2 border-t border-white/10"></div>

          <router-link
            to="/admin/profile-settings"
            exact-active-class="nav-active"
            class="block rounded-lg px-3 py-3 text-base font-medium text-white/90 transition hover:bg-white/10 hover:text-white"
            @click="closeMobileMenu"
          >
            Configuración
          </router-link>
        </nav>
      </div>
    </Transition>

    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="-translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="-translate-y-2 opacity-0"
    >
      <div
        v-if="mobileMenuOpen"
        class="border-t border-white/10 bg-primary lg:hidden"
      >
        <nav class="mx-auto max-w-7xl space-y-1 px-4 py-3 sm:px-6">
          <router-link
            v-for="item in menuItems"
            :key="item.route"
            :to="item.route"
            exact-active-class="nav-active"
            class="block rounded-lg px-3 py-3 text-base font-medium text-white/90 transition hover:bg-white/10 hover:text-white"
            @click="closeMobileMenu"
          >
            {{ item.label }}
          </router-link>

          <div class="my-2 border-t border-white/10"></div>

          <router-link
            to="/admin/profile-settings"
            exact-active-class="nav-active"
            class="block rounded-lg px-3 py-3 text-base font-medium text-white/90 transition hover:bg-white/10 hover:text-white"
            @click="closeMobileMenu"
          >
            Configuración
          </router-link>
        </nav>
      </div>
    </Transition>
  </header>

  <ErrorModal
    v-model="showMaintenanceModal"
    title="En Mantenimiento"
    message="El módulo de Reportes se encuentra actualmente en mantenimiento. Por favor, intente más tarde."
    buttonText="Entendido"
  />

  <ErrorModal
    v-model="showMaintenanceModal"
    title="En Mantenimiento"
    message="El módulo de Reportes se encuentra actualmente en mantenimiento. Por favor, intente más tarde."
    buttonText="Entendido"
  />
</template>

<script setup>
import { ref } from 'vue'
import { Bars3Icon, XMarkIcon } from '@heroicons/vue/24/outline'
import ErrorModal from './ErrorModal.vue'

const mobileMenuOpen = ref(false)
const showMaintenanceModal = ref(false)

const menuItems = [
  { route: '/admin/aircrafts', label: 'Aeronaves' },
  { route: '/admin/airports', label: 'Aeropuertos' },
  { route: '/admin/routes', label: 'Rutas' },
  { route: '/admin/users', label: 'Usuarios' },
  { route: '/admin/reports', label: 'Reportes' }
]

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value
}

const closeMobileMenu = () => {
  mobileMenuOpen.value = false
}
<script setup>
import { ref } from 'vue'
import { Bars3Icon, XMarkIcon } from '@heroicons/vue/24/outline'
import ErrorModal from './ErrorModal.vue'

const mobileMenuOpen = ref(false)
const showMaintenanceModal = ref(false)

const menuItems = [
  { route: '/admin/aircrafts', label: 'Aeronaves' },
  { route: '/admin/airports', label: 'Aeropuertos' },
  { route: '/admin/routes', label: 'Rutas' },
  { route: '/admin/users', label: 'Usuarios' },
  { route: '/admin/reports', label: 'Reportes' }
]

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value
}

const closeMobileMenu = () => {
  mobileMenuOpen.value = false
}
</script>

<style scoped>
.nav-active {
  background-color: rgb(0 0 0 / 0.15);
}
<style scoped>
.nav-active {
  background-color: rgb(0 0 0 / 0.15);
}
</style>