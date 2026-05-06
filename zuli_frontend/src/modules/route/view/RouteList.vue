<script setup>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import FlightRouteComponent from "../components/FlightRouteComponent.vue";
import RouteNavBar from "../components/RouteNavBar.vue";
</script>

<template>
  <div class="flex flex-col">
    <PublicNavBar />
    <RouteNavBar/>
    <main class="flex-1 pb-8">
      <div class="page-shell">
        <FlightRouteComponent />
      </div>
    </main>
  </div>
</template>
