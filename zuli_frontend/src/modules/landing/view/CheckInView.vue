<template>
  <div class="landing-page">
    <PublicNavBar />
<<<<<<< HEAD
    <h1 class = "message">Check-in en mantenimiento</h1>
    <PublicBottomBar />
  </div>
  
</template>

<script>
import PublicNavBar from '../components/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
=======
    <h1 class = "message">Airports en mantenimiento</h1>
  </div>
</template>

<script>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
>>>>>>> develop

export default {
  components: {
    PublicNavBar
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