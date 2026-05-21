<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import AircraftComponent from '../components/AircraftComponent.vue';
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AircraftComponent/>
            </div>
        </main>
    </div>
</template>