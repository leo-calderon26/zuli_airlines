<script setup>
import PublicNavBar from '../../../components/PublicNavBar.vue'
import PublicBottomBar from '../../../components/PublicBottomBar.vue'
import HeaderComponent from '../../../shared/HeaderComponent.vue';
import AircraftComponent from '../components/AircraftComponent.vue';
import AircraftNavBar from '../components/AircraftNavBar.vue';
</script>

<template>
    <PublicNavBar/>
    <AircraftNavBar/>
    <div class="flex">
    <div class="size-14 grow-2"></div>
    <div class="size-14 grow-9">
        <AircraftComponent/>
    </div>
    <div class="size-14 grow-2"></div>
    </div>

</template>