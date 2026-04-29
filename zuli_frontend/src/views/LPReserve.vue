
<template>
    <div class = "landing-page">
        <PublicNavBar />
        <main class="reserve-card-center">
            <ReserveCard />
        </main>
        <PublicBottomBar />
    </div>
</template>

<script>
import PublicNavBar from '../components/PublicNavBar.vue';
import PublicBottomBar from '../components/PublicBottomBar.vue';
import ReserveCard from '../components/ReserveCard.vue';
    export default {
        components: {
            PublicNavBar,
            PublicBottomBar,
            ReserveCard
        }
    }
</script>

<style scoped>
.landing-page {
  min-height: 100svh;
  display: flex;
  flex-direction: column;
}

.reserve-card-center {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 1rem;
    transform: translateY(-30px);
}

</style>