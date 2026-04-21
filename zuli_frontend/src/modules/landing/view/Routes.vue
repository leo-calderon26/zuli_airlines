<template>
  <div class="landing-page">
    <PublicNavBar />
    <h1 class = "message">Routes en mantenimiento</h1>
    <PublicBottomBar />
  </div>
</template>

<script>
import PublicNavBar from '../components/PublicNavBar.vue'
import PublicBottomBar from '../components/PublicBottomBar.vue'

export default {
  components: {
    PublicNavBar,
    PublicBottomBar
  }
}
</script>

<style scoped>
.landing-page {
  min-height: 100svh;
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