<template>
  <header class="w-full bg-primary text-white">
    <div class="relative flex w-full items-center px-4 py-2 sm:px-6">
      <img src="../../../assets/logoZuli.svg" alt="Logo" class="h-14 w-auto shrink-0 sm:h-20" />

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
            <router-link
              to="/check-in"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Check-in
            </router-link>
          </li>
          <li class="px-4">
            <router-link
              to="/consulta"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Consulta
            </router-link>
          </li>
          <li class="px-4">
            <router-link
              to="/ayuda"
              exact-active-class="nav-active"
              class="inline-block rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
            >
              Ayuda
            </router-link>
          </li>
        </ul>
      </nav>

      <router-link
        to="/administrativo"
        class="ml-auto inline-flex items-center gap-2 rounded-md px-3 py-2 font-semibold transition hover:bg-black/15"
      >
        <img src="../../../assets/person.svg" alt="Admin Icon" class="h-6 w-6 shrink-0" />
        <span>Panel administrativo</span>
      </router-link>
    </div>
  </header>
</template>

<script>
    export default {
        
    }
</script>

<style scoped>
.nav-active {
  background-color: rgba(0, 0, 0, 0.25);
}
</style>
