<template>
    <div class="landing-page">
        <PublicNavBar />
        <h1 class="message">Profile settings en mantenimiento</h1>
        <PublicBottomBar />
    </div>
    <PublicBottomBar />
</template>

<script>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';

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