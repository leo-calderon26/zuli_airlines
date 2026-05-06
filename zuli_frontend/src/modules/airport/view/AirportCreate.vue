<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import AirportNavBar from '../components/AirportNavBar.vue';
</script>

<template>
    <div class="flex flex-col min-h-screen">
        <PublicNavBar/>   
        <AirportNavBar/>
        <PublicBottomBar />
    </div>
</template>