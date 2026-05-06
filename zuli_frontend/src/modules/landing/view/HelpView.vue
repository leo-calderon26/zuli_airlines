<template>
  <div class="landing-page">
    <PublicNavBar />
    <h1 class = "message">Routes en mantenimiento</h1>
  </div>
</template>

<script>
import PublicNavBar from '../../../shared/PublicNavBar.vue';

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