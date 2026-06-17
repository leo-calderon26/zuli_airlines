<script setup>
import { ref, computed } from 'vue';
import { RouterView, useRoute } from 'vue-router';
import AdminNavBar from './shared/AdminNavBar.vue';
import PublicNavBar from './shared/PublicNavBar.vue';
import PublicBottomBar from './shared/PublicBottomBar.vue';
import AdminSidebar from './shared/components/layout/AdminSidebar.vue';
import AdminTopBar from './shared/components/layout/AdminTopBar.vue';

const route = useRoute();
const sidebarOpen = ref(true);

const noNavBarRoutes = ['administrativo', 'purchaseConfirmation'];

const isReportsSection = computed(() => {
  return route.path.startsWith('/admin/reports');
});

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

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value;
};

const closeSidebar = () => {
  sidebarOpen.value = false;
};
</script>

<template>
  <div class="min-h-screen flex flex-col bg-secondary">

    <AdminNavBar v-if="showAdminNavBar" />
    <PublicNavBar v-if="showPublicNavBar" />

    <div v-if="isReportsSection" class="flex flex-1 bg-[#F7F3F2]">
      <div
        class="transition-all duration-300 overflow-hidden flex-shrink-0 z-40"
        :class="sidebarOpen ? 'w-64' : 'w-0'"
      >
        <AdminSidebar />
      </div>
      <div class="flex-1 flex flex-col min-w-0">
        <AdminTopBar @toggle-sidebar="toggleSidebar" />
        <main class="flex-1 overflow-auto p-6">
          <RouterView />
        </main>
      </div>
    </div>

    <RouterView v-if="!isReportsSection" v-slot="{ Component }">
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
