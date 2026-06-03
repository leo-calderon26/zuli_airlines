<script setup>
import { computed } from 'vue';
import { RouterView, useRoute } from 'vue-router';
import AdminNavBar from './shared/AdminNavBar.vue';
import PublicNavBar from './shared/PublicNavBar.vue';
import PublicBottomBar from './shared/PublicBottomBar.vue';

const route = useRoute();

const noNavBarRoutes = ['administrativo', 'purchaseConfirmation'];

const showAdminNavBar = computed(() => {
  if (noNavBarRoutes.includes(route.name)) return false;
  
  return route.path.startsWith('/admin') || 
         route.name === 'unauthorizedAccess' || 
         route.name === 'userActivation';
});

const showPublicNavBar = computed(() => {
  if (noNavBarRoutes.includes(route.name)) return false;
  
  return !showAdminNavBar.value;
});
</script>

<template>
  <div class="min-h-screen flex flex-col bg-secondary">
    
    <AdminNavBar v-if="showAdminNavBar" />
    <PublicNavBar v-if="showPublicNavBar" />

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