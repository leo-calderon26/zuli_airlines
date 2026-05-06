<template>
  <div class="flex flex-col">
    <PublicNavBar />
    <AirportNavBar/>
    <main class="flex-1">
        <div class="page-shell">
            <h1>Aqui es donde se va ver la lista de aeropuertos</h1>
        </div>
    </main>
  </div>
</template>

<script>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import AirportNavBar from '../components/AirportNavBar.vue';

export default {
  components: {
    PublicNavBar,
    AirportNavBar
  }
}
</script>

<style scoped>
.landing-page {
  display: flex;
  flex-direction: column;
}

.landing-page > :last-child {
  margin-top: auto;
}

.message {
  color: #000;
}
</style>