<script setup>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import FlightRouteComponent from "../components/FlightRouteComponent.vue";
</script>

<template>
  <div class="flex flex-col">
    <PublicNavBar />
    <main class="flex-1 pb-8">
      <div class="page-shell">
        <FlightRouteComponent />
      </div>
    </main>
  </div>
</template>
