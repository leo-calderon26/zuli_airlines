<script setup>
  import { RouterView } from 'vue-router';
  import PublicBottomBar from './shared/PublicBottomBar.vue';
</script>

<template>
  <div class="min-h-screen flex flex-col">
    <RouterView v-slot="{ Component }">
      <component :is="Component" class="flex flex-col flex-1" />
    </RouterView>
    <PublicBottomBar />
  </div>
</template>

<script>
export default {
    name: "App"
};
</script>